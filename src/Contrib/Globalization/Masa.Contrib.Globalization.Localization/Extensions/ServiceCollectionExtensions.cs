// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaLocalization(this IServiceCollection services, Action<MasaLocalizationOptions>? action = null)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton(typeof(IMasaStringLocalizer<>), typeof(MasaStringLocalizer<>));
        services.AddSingleton<IMasaStringLocalizerFactory, MasaStringLocalizerFactory>();

        if (action != null)
        {
            services.Configure(action);
        }

        _ = services.BuildServiceProvider().GetRequiredService<IOptions<MasaLocalizationOptions>>();
        services.AddLocalization();
        return services;
    }
}
