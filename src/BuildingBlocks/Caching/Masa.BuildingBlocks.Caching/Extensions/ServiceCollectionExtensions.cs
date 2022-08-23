// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCachingCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IDistributedCacheClientFactory, DefaultDistributedCacheClientFactory>();
        services.TryAddSingleton<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        return services;
    }
}
