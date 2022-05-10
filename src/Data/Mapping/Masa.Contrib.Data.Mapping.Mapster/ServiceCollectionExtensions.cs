// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
        => services.AddMapping(MapMode.Shared);

    public static IServiceCollection AddMapping(this IServiceCollection services, MapMode mode)
        => services.AddMapping(new MapOptions()
        {
            Mode = mode
        });

    public static IServiceCollection AddMapping(this IServiceCollection services, MapOptions mapOptions)
    {
        if (services.Any(service => service.ImplementationType == typeof(MappingProvider)))
            return services;

        services.AddSingleton<MappingProvider>();

        services.TryAddSingleton<IMappingConfigProvider>(_ => new DefaultMappingConfigProvider(mapOptions));
        services.TryAddSingleton<IMapper, DefaultMapper>();
        return services;
    }

    private class MappingProvider
    {
    }
}
