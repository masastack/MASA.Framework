// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18NOptions>? action = null)
        => services.AddI18NByEmbedded(Array.Empty<Assembly>(), languageDirectory, supportCultureName, action);

    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction = null,
        Action<I18NOptions>? action = null)
        => services.AddI18NByEmbedded(Array.Empty<Assembly>(), settingsAction, action);

    public static IServiceCollection AddI18NByEmbedded(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18NOptions>? action = null)
    {
        return services.AddI18NByEmbedded(assemblies,
            settings =>
            {
                settings.ResourcesDirectory = languageDirectory;
                settings.SupportCultureName = supportCultureName.IsNullOrWhiteSpace() ? ContribI18NConstant.SUPPORTED_CULTURES_NAME :
                    supportCultureName;
            }, action);
    }

    public static IServiceCollection AddI18NByEmbedded(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<CultureSettings>? settingsAction = null,
        Action<I18NOptions>? action = null)
    {
        MasaApp.TrySetServiceCollection(services);

        var cultureSettings = AddAndGetCultureSettings(services, settingsAction);
        return services
            .AddI18NByFramework(cultureSettings)
            .AddI18NCore<DefaultResource>(action, assemblies, cultureSettings);
    }

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        string languageDirectory,
        string? supportCultureName = null,
        Action<I18NOptions>? action = null)
    {
        return services.TestAddI18N(settings =>
        {
            settings.ResourcesDirectory = languageDirectory;
            settings.SupportCultureName = supportCultureName.IsNullOrWhiteSpace() ? ContribI18NConstant.SUPPORTED_CULTURES_NAME :
                supportCultureName;
            settings.SupportedCultures = CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureName!);

            if (string.IsNullOrEmpty(settings.DefaultCulture))
                settings.DefaultCulture = settings.SupportedCultures.Select(c => c.Culture).FirstOrDefault()!;
        }, action);
    }

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction = null,
        Action<I18NOptions>? action = null)
    {
        MasaApp.SetServiceCollection(services);
        var cultureSettings = AddAndGetCultureSettings(services, settingsAction);
        return services
            .AddI18NByFramework(cultureSettings)
            .AddI18NCore<DefaultResource>(action, Array.Empty<Assembly>(), cultureSettings);
    }

    private static IServiceCollection AddI18NByFramework(this IServiceCollection services, CultureSettings languageSettings)
    {
        services.Configure<MasaI18NOptions>(options =>
        {
            var assembly = typeof(EmbeddedResourceUtils).Assembly;
            options.Resources.TryAdd<MasaFrameworkResource>(resource =>
            {
                resource.AddJsonByEmbeddedResource(new[] { assembly },
                    ContribI18NConstant.DefaultFrameworkResourcePath,
                    languageSettings.SupportedCultures);
            });

            options.Resources.TryAdd<MasaParameterValidationResource>(resource =>
            {
                resource.AddJsonByEmbeddedResource(new[] { assembly },
                    ContribI18NConstant.DefaultFrameworkParameterValidationResourcePath,
                    languageSettings.SupportedCultures);
            });

            options.Resources.TryAdd<MasaLanguageResource>(resource =>
            {
                resource.AddJsonByEmbeddedResource(new[] { assembly },
                    ContribI18NConstant.DefaultFrameworkLanguageResourcePath,
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
                settings.ResourcesDirectory = ContribI18NConstant.DefaultResourcePath;

            if (string.IsNullOrWhiteSpace(settings.SupportCultureName))
                settings.SupportCultureName = ContribI18NConstant.SUPPORTED_CULTURES_NAME;

            if (!settings.SupportedCultures.Any())
                settings.SupportedCultures =
                    CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureName);

            if (string.IsNullOrEmpty(settings.DefaultCulture))
                settings.DefaultCulture = settings.SupportedCultures.Select(c => c.Culture).FirstOrDefault()!;
        });
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IOptions<CultureSettings>>().Value;
    }

    private static IServiceCollection AddI18NCore<TResource>(
        this IServiceCollection services,
        Action<I18NOptions>? action,
        IEnumerable<Assembly> assemblies,
        CultureSettings cultureSettings) where TResource : class
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18N<>), typeof(I18NOfT<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();
        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService<II18N<DefaultResource>>());

        services.Configure<MasaI18NOptions>(options =>
        {
            var localLanguageSettings = cultureSettings;
            options.Resources.TryAdd<TResource>(resource =>
            {
                if (!assemblies.Any())
                {
                    resource.AddJson(
                        localLanguageSettings.ResourcesDirectory ?? ContribI18NConstant.DefaultResourcePath,
                        localLanguageSettings.SupportedCultures);
                }
                else
                {
                    resource.AddJsonByEmbeddedResource(assemblies,
                        localLanguageSettings.ResourcesDirectory ?? ContribI18NConstant.DefaultResourcePath,
                        localLanguageSettings.SupportedCultures);
                }
            });
        });

        action?.Invoke(new I18NOptions(services, cultureSettings.SupportedCultures));

        var i18NOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaI18NOptions>>();
        foreach (var resource in i18NOptions.Value.Resources)
            I18NResourceResourceConfiguration.Resources[resource.Key] = resource.Value;

        return services;
    }
}
