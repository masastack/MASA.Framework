// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static Task<IServiceCollection> AddI18NForBlazorWebAssemblyAsync(
        this IServiceCollection services,
        string baseAddress,
        Action<I18NOptions>? action = null)
        => services.AddI18NForBlazorWebAssemblyAsync(settings =>
        {
            settings.ResourcesDirectory = baseAddress;
        }, action);

    public static Task<IServiceCollection> AddI18NForBlazorWebAssemblyAsync(
        this IServiceCollection services,
        Action<CultureSettings> settingsAction,
        Action<I18NOptions>? action = null)
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18N<>), typeof(Masa.Contrib.Globalization.I18N.BlazorWebAssembly.I18NOfT<>));
        services.TryAddScoped<ILanguageProvider, DefaultLanguageProvider>();

        MasaApp.TrySetServiceCollection(services);
        return services.AddI18NForBlazorWebAssemblyCoreAsync(settingsAction, action);
    }

    private static async Task<IServiceCollection> AddI18NForBlazorWebAssemblyCoreAsync(
        this IServiceCollection services,
        Action<CultureSettings>? settingsAction,
        Action<I18NOptions>? action = null)
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18N<>), typeof(Masa.Contrib.Globalization.I18N.BlazorWebAssembly.I18NOfT<>));

        var languageSettings = await GetCultureSettingsAsync(settingsAction);
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(languageSettings));

        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService<II18N<DefaultResource>>());

        var serviceProvider = services.BuildServiceProvider();

        await I18NResourceResourceConfiguration.Resources.TryAddAsync<DefaultResource>(async resource
            => await resource.AddJsonAsync(languageSettings.ResourcesDirectory!, BlazorWebAssemblyConstant.DefaultResourcePath,
                languageSettings.SupportedCultures));
        action?.Invoke(new I18NOptions(services, languageSettings.SupportedCultures));

        var i18N = serviceProvider.GetRequiredService<II18N>();
        CultureInfo.CurrentCulture = i18N.GetCultureInfo();
        CultureInfo.CurrentUICulture = i18N.GetUiCultureInfo();
        return services;
    }

    private static async Task<CultureSettings> GetCultureSettingsAsync(Action<CultureSettings>? settingsAction)
    {
        var cultureSettings = new CultureSettings();
        settingsAction?.Invoke(cultureSettings);

        ArgumentNullException.ThrowIfNull(cultureSettings.ResourcesDirectory);

        if (string.IsNullOrWhiteSpace(cultureSettings.SupportCultureName))
            cultureSettings.SupportCultureName = BlazorWebAssemblyConstant.SUPPORTED_CULTURES_NAME;

        if (!cultureSettings.SupportedCultures.Any())
            cultureSettings.SupportedCultures = await CultureUtils
                .GetSupportedCulturesAsync(cultureSettings.ResourcesDirectory!, BlazorWebAssemblyConstant.DefaultResourcePath,
                    cultureSettings.SupportCultureName);

        if (string.IsNullOrEmpty(cultureSettings.DefaultCulture))
            cultureSettings.DefaultCulture = cultureSettings.SupportedCultures.Select(c => c.Culture).FirstOrDefault()!;
        return cultureSettings;
    }
}
