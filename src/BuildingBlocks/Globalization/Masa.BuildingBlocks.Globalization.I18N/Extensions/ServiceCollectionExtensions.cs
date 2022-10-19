// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddI18N(
        this IServiceCollection services,
        Action<I18NOptions>? action)
    {
        MasaApp.TrySetServiceCollection(services);
        return services.AddI18NCore(action);
    }

    public static IServiceCollection TestAddI18N(
        this IServiceCollection services,
        Action<I18NOptions>? action)
    {
        MasaApp.SetServiceCollection(services);
        return services.AddI18NCore(action);
    }

    private static IServiceCollection AddI18NCore(
        this IServiceCollection services,
        Action<I18NOptions>? action)
    {
        services.TryAddTransient(typeof(II18N<>), typeof(I18N<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        action?.Invoke(new I18NOptions());

        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService<II18N<DefaultResource>>());

        var i18NOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaI18NOptions>>();
        foreach (var resource in i18NOptions.Value.Resources)
        {
            I18NResourceResourceConfiguration.Resources[resource.Key] = resource.Value;
        }

        return services;
    }
}
