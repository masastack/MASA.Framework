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
        services.Configure<MasaI18NOptions>(options =>
        {
            options.DefaultResourceType ??= typeof(DefaultResource);
        });
        services.TryAddTransient(typeof(II18N<>), typeof(I18N<>));
        services.TryAddSingleton<ILanguageProvider, DefaultLanguageProvider>();

        action?.Invoke(new I18NOptions());

        var masaLocalizationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaI18NOptions>>().Value;
        var defaultResourceLocalizer = typeof(II18N<>).MakeGenericType(masaLocalizationOptions.DefaultResourceType!);
        services.TryAddTransient(serviceProvider => (II18N)serviceProvider.GetRequiredService(defaultResourceLocalizer));
        return services;
    }
}
