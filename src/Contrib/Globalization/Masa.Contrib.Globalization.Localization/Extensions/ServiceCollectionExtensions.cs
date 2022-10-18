// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaLocalization(
        this IServiceCollection services,
        Action<Masa.Contrib.Globalization.Localization.LocalizationOptions>? action)
    {
        MasaApp.TrySetServiceCollection(services);
        return services.AddMasaLocalizationCore(action);
    }

    public static IServiceCollection TestAddMasaLocalization(
        this IServiceCollection services,
        Action<Masa.Contrib.Globalization.Localization.LocalizationOptions>? action)
    {
        MasaApp.SetServiceCollection(services);
        return services.AddMasaLocalizationCore(action);
    }

    private static IServiceCollection AddMasaLocalizationCore(this IServiceCollection services,
        Action<Masa.Contrib.Globalization.Localization.LocalizationOptions>? action)
    {
        services.Configure<MasaLocalizationOptions>(options =>
        {
            options.DefaultResourceType ??= typeof(DefaultResource);
        });
        services.TryAddSingleton(typeof(IMasaStringLocalizer<>), typeof(MasaStringLocalizer<>));
        services.AddSingleton<IMasaStringLocalizerFactory, MasaStringLocalizerFactory>();

        action?.Invoke(new Masa.Contrib.Globalization.Localization.LocalizationOptions());

        var masaLocalizationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MasaLocalizationOptions>>().Value;
        var defaultResourceLocalizer = typeof(IMasaStringLocalizer<>).MakeGenericType(masaLocalizationOptions.DefaultResourceType!);
        services.TryAddSingleton(serviceProvider => (IMasaStringLocalizer)serviceProvider.GetRequiredService(defaultResourceLocalizer));
        return services;
    }
}
