// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18n(
        this IServiceCollection services,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18nOptions>? action = null)
        => services.AddI18nByEmbedded(Array.Empty<Assembly>(), languageDirectory, supportCultureName, action);

    public static IServiceCollection AddI18n(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction = null,
        Action<I18nOptions>? action = null)
        => services.AddI18nByEmbedded(Array.Empty<Assembly>(), settingsAction, action);

    public static IServiceCollection AddI18nByEmbedded(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18nOptions>? action = null)
    {
        return services.AddI18nByEmbedded(assemblies,
            settings =>
            {
                settings.ResourcesDirectory = languageDirectory;
                settings.SupportCultureFileName = supportCultureName.IsNullOrWhiteSpace() ? ContribI18nConstant.SUPPORTED_CULTURES_NAME :
                    settings.GetSupportCultureFileName();
            }, action);
    }

    public static IServiceCollection AddI18nByEmbedded(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<CultureSettings>? settingsAction = null,
        Action<I18nOptions>? action = null)
    {
        MasaApp.TrySetServiceCollection(services);

        var cultureSettings = AddAndGetCultureSettings(services, settingsAction);
        return services
            .AddI18nByFramework(cultureSettings)
            .AddI18nCore<DefaultResource>(action, assemblies, cultureSettings);
    }

    public static IServiceCollection TestAddI18n(
        this IServiceCollection services,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18nOptions>? action = null)
    {
        return services.TestAddI18n(settings =>
        {
            settings.ResourcesDirectory = languageDirectory;
            settings.SupportCultureFileName = supportCultureName.IsNullOrWhiteSpace() ? ContribI18nConstant.SUPPORTED_CULTURES_NAME :
                supportCultureName;
            settings.SupportedCultures = CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureFileName!);
        }, action);
    }

    public static IServiceCollection TestAddI18n(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction = null,
        Action<I18nOptions>? action = null)
    {
        MasaApp.SetServiceCollection(services);
        var cultureSettings = AddAndGetCultureSettings(services, settingsAction);
        return services
            .AddI18nByFramework(cultureSettings)
            .AddI18nCore<DefaultResource>(action, Array.Empty<Assembly>(), cultureSettings);
    }

    private static IServiceCollection AddI18nByFramework(this IServiceCollection services, CultureSettings languageSettings)
    {
        services.Configure<MasaI18nOptions>(options =>
        {
            var assembly = typeof(EmbeddedResourceUtils).Assembly;
            options.Resources.TryAdd<MasaFrameworkResource>(resource =>
            {
                resource.Assemblies = new[]
                {
                    assembly
                };
                resource.AddJson(ContribI18nConstant.DefaultFrameworkResourcePath,
                    languageSettings.SupportedCultures);
            });

            options.Resources.TryAdd<MasaParameterValidationResource>(resource =>
            {
                resource.Assemblies = new[]
                {
                    assembly
                };
                resource.AddJson(ContribI18nConstant.DefaultFrameworkParameterValidationResourcePath,
                    languageSettings.SupportedCultures);
            });

            options.Resources.TryAdd<MasaLanguageResource>(resource =>
            {
                resource.Assemblies = new[]
                {
                    assembly
                };
                resource.AddJson(ContribI18nConstant.DefaultFrameworkLanguageResourcePath,
                    languageSettings.SupportedCultures);
            });

            options.Resources.TryAdd<MasaBackgroundJobResource>(resource =>
            {
                resource.Assemblies = new[]
                {
                    assembly
                };
                resource.AddJson(ContribI18nConstant.DefaultFrameworkBackgroundJobResourcePath,
                    languageSettings.SupportedCultures);
            });
        });
        return services;
    }

    private static CultureSettings AddAndGetCultureSettings(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction)
    {
        services.Configure<CultureSettings>(settings =>
        {
            settingsAction?.Invoke(settings);

            if (string.IsNullOrWhiteSpace(settings.ResourcesDirectory))
                settings.ResourcesDirectory = ContribI18nConstant.DefaultResourcePath;

            settings.SupportCultureFileName =  settings.GetSupportCultureFileName();

            if (!settings.SupportedCultures.Any())
                settings.SupportedCultures =
                    CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureFileName);
        });
        var serviceProvider = services.BuildServiceProvider();
        var settings = serviceProvider.GetRequiredService<IOptions<CultureSettings>>().Value;
        GlobalI18nConfiguration.SetSupportedCultures(settings.SupportedCultures);
        return settings;
    }

    private static IServiceCollection AddI18nCore<TResource>(
        this IServiceCollection services,
        Action<I18nOptions>? action,
        IEnumerable<Assembly> assemblies,
        CultureSettings cultureSettings) where TResource : class
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18n<>), typeof(I18n<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();
        services.TryAddTransient(serviceProvider => (II18n)serviceProvider.GetRequiredService<II18n<DefaultResource>>());

        services.Configure<MasaI18nOptions>(options =>
        {
            var localLanguageSettings = cultureSettings;
            options.Resources.TryAdd<TResource>(resource =>
            {
                if (assemblies.Any()) resource.Assemblies = assemblies;
                resource.AddJson(localLanguageSettings.ResourcesDirectory!,
                    localLanguageSettings.SupportedCultures);
            });
        });

        action?.Invoke(new I18nOptions(services, cultureSettings.SupportedCultures));

        var i18nOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaI18nOptions>>();
        foreach (var resource in i18nOptions.Value.Resources)
            I18nResourceResourceConfiguration.Resources[resource.Key] = resource.Value;

        return services;
    }
}
