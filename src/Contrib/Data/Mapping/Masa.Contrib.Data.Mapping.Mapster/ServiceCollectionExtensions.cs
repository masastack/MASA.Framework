// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
        => services.AddMapster(MapMode.Shared);

    public static IServiceCollection AddMapster(this IServiceCollection services, MapMode mode)
        => services.AddMapster(new MapOptions()
        {
            Mode = mode
        });

    public static IServiceCollection AddMapster(this IServiceCollection services, MapOptions mapOptions)
    {
        if (services.Any(service => service.ImplementationType == typeof(MappingProvider)))
            return services;

        services.AddSingleton<MappingProvider>();

        services.TryAddSingleton<IMappingConfigProvider>(_ => new DefaultMappingConfigProvider(mapOptions));
        services.TryAddSingleton<IMapper, DefaultMapper>();

        return services;
    }

    [Obsolete("Use AddMapster instead")]
    public static IServiceCollection AddMapping(this IServiceCollection services)
        => services.AddMapster(MapMode.Shared);

    [Obsolete("Use AddMapster instead")]
    public static IServiceCollection AddMapping(this IServiceCollection services, MapMode mode)
        => services.AddMapster(mode);

    [Obsolete("Use AddMapster instead")]
    public static IServiceCollection AddMapping(this IServiceCollection services, MapOptions mapOptions)
        => services.AddMapster(mapOptions);

    private class MappingProvider
    {
    }
}
