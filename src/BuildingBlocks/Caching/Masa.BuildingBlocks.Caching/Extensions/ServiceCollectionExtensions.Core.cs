// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching.Extensions;

/// <summary>
/// It is only used for inheritance by Contrib implementation, and does not support the extension of IServiceCollection to avoid reference barriers
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddDistributedCacheCore(IServiceCollection services)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IDistributedCacheClientFactory, DistributedCacheClientFactoryBase>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        return services;
    }

    public static IServiceCollection TryAddMultilevelCacheCore(IServiceCollection services)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IMultilevelCacheClientFactory, MultilevelCacheClientFactoryBase>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        return services;
    }
}
