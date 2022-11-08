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
    {
        return services.AddI18N(settings =>
        {
            settings.ResourcesDirectory = languageDirectory;
            settings.SupportCultureName = supportCultureName.IsNullOrWhiteSpace() ? Constant.SUPPORTED_CULTURES_NAME : supportCultureName;
            settings.SupportedCultures = CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureName!);

            if (string.IsNullOrEmpty(settings.DefaultCulture))
                settings.DefaultCulture = settings.SupportedCultures.Select(c => c.Culture).FirstOrDefault()!;
        }, action);
    }

    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction = null,
        Action<I18NOptions>? action = null)
    {
        MasaApp.TrySetServiceCollection(services);
        return services.AddI18NCore(settingsAction, action);
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
            settings.SupportCultureName = supportCultureName.IsNullOrWhiteSpace() ? Constant.SUPPORTED_CULTURES_NAME : supportCultureName;
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
        return services.AddI18NCore(settingsAction, action);
    }

    private static IServiceCollection AddI18NCore(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction,
        Action<I18NOptions>? action = null)
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18N<>), typeof(I18NOfT<>));
        services.Configure<CultureSettings>(settings =>
        {
            settingsAction?.Invoke(settings);

            if (string.IsNullOrWhiteSpace(settings.ResourcesDirectory))
                settings.ResourcesDirectory = Constant.DefaultResourcePath;

            if (string.IsNullOrWhiteSpace(settings.SupportCultureName))
                settings.SupportCultureName = Constant.SUPPORTED_CULTURES_NAME;

            if (!settings.SupportedCultures.Any())
                settings.SupportedCultures =
                    CultureUtils.GetSupportedCultures(settings.ResourcesDirectory, settings.SupportCultureName);

            if (string.IsNullOrEmpty(settings.DefaultCulture))
                settings.DefaultCulture = settings.SupportedCultures.Select(c => c.Culture).FirstOrDefault()!;
        });

        CultureSettings? languageSettings = null;
        services.Configure<MasaI18NOptions>(options =>
        {
            var localLanguageSettings = languageSettings;
            if (localLanguageSettings == null)
            {
                var serviceProvider = MasaApp.GetServices().BuildServiceProvider();
                localLanguageSettings = serviceProvider.GetService<IOptions<CultureSettings>>()?.Value ?? new CultureSettings();
                languageSettings ??= localLanguageSettings;
            }

            options.Resources.TryAdd<MasaFrameworkResource>(resource
                    => resource.AddJson(Constant.DefaultFrameworkResourcePath, localLanguageSettings.SupportedCultures));

            options.Resources.TryAdd<MasaExceptionResource>(resource
                => resource.AddJson(Constant.DefaultFrameworkExceptionResourcePath, localLanguageSettings.SupportedCultures));

            options.Resources.TryAdd<MasaLanguageResource>(resource
                => resource.AddJson(Constant.DefaultFrameworkLanguageResourcePath, localLanguageSettings.SupportedCultures));

            options.Resources.TryAdd<DefaultResource>(resource
                => resource.AddJson(localLanguageSettings.ResourcesDirectory ?? Constant.DefaultResourcePath,
                    localLanguageSettings.SupportedCultures));
        });

        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService<II18N<DefaultResource>>());

        var serviceProvider = services.BuildServiceProvider();
        if (action != null)
        {
            languageSettings = serviceProvider.GetRequiredService<IOptions<CultureSettings>>().Value;
            action.Invoke(new I18NOptions(services, languageSettings.SupportedCultures));
        }

        var i18NOptions = serviceProvider.GetRequiredService<IOptions<MasaI18NOptions>>();
        foreach (var resource in i18NOptions.Value.Resources)
            I18NResourceResourceConfiguration.Resources[resource.Key] = resource.Value;
        return services;
    }
}
