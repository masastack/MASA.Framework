// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
        => services.AddI18N(
            Masa.Contrib.Globalization.I18N.Internal.Constant.DEFAULT_RESOURCE_PATH,
            Masa.Contrib.Globalization.I18N.Internal.Constant.SUPPORTED_CULTURES_NAME,
            action,
            languages);

    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        string languageDirectory,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
        => services.AddI18N(
            languageDirectory,
            Masa.Contrib.Globalization.I18N.Internal.Constant.SUPPORTED_CULTURES_NAME,
            action,
            languages);

    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        string languageDirectory,
        string supportCultureName,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
    {
        MasaApp.TrySetServiceCollection(services);
        return services.AddI18NCore(languageDirectory, supportCultureName, languages);
    }

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
        => services.TestAddI18N(
            Masa.Contrib.Globalization.I18N.Internal.Constant.DEFAULT_RESOURCE_PATH,
            Masa.Contrib.Globalization.I18N.Internal.Constant.SUPPORTED_CULTURES_NAME,
            action,
            languages);

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        string languageDirectory,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
        => services.TestAddI18N(
            languageDirectory,
            Masa.Contrib.Globalization.I18N.Internal.Constant.SUPPORTED_CULTURES_NAME,
            action,
            languages);

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        string languageDirectory,
        string supportCultureName,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
    {
        MasaApp.SetServiceCollection(services);
        services.AddI18NCore(languageDirectory, supportCultureName, languages);
        I18NOptions i18NOptions = new I18NOptions(services);
        action?.Invoke(i18NOptions);
        return services;
    }

    private static IServiceCollection AddI18NCore(
        this IServiceCollection services,
        string languageDirectory,
        string supportCultureName,
        params LanguageInfo[] languages)
    {
        services.AddOptions();
        services.TryAddTransient(typeof(II18N<>), typeof(I18N<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService<II18N<DefaultResource>>());

        var i18NOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaI18NOptions>>();
        foreach (var resource in i18NOptions.Value.Resources)
        {
            I18NResourceResourceConfiguration.Resources[resource.Key] = resource.Value;
        }
        JsonConfigurationUtils.AddJson(languageDirectory, supportCultureName, languages);
        return services;
    }
}
