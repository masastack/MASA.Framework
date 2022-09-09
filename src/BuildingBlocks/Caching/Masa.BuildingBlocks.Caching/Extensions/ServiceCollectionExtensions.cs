﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCacheCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IDistributedCacheClientFactory, DefaultDistributedCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        return services;
    }

    public static IServiceCollection AddMultilevelCacheCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        return services;
    }
}