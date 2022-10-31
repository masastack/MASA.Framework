// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18NForBlazor(this IServiceCollection services,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
    {
        services.TryAddTransient(typeof(II18N<>), typeof(Masa.Contrib.Globalization.I18N.Blazor.I18N<>));
        services.AddI18N(action, languages);
        return services;
    }

    public static IServiceCollection AddI18NForBlazor(
        this IServiceCollection services,
        string languageDirectory,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
    {
        services.TryAddTransient(typeof(II18N<>), typeof(I18N<>));
        return services.AddI18N(languageDirectory, action, languages);
    }

    public static IServiceCollection AddI18NForBlazor(
        this IServiceCollection services,
        string languageDirectory,
        string supportCultureName,
        Action<I18NOptions>? action = null,
        params LanguageInfo[] languages)
    {
        services.TryAddTransient(typeof(II18N<>), typeof(I18N<>));
        services.AddI18N(languageDirectory, supportCultureName, action, languages);
        return services;
    }
}
