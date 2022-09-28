// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddDistributedCacheCore(this IServiceCollection services)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IDistributedCacheClientFactory, DistributedCacheClientFactoryBase>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        return services;
    }

    public static IServiceCollection TryAddMultilevelCacheCore(this IServiceCollection services)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IMultilevelCacheClientFactory, MultilevelCacheClientFactoryBase>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        return services;
    }
}
