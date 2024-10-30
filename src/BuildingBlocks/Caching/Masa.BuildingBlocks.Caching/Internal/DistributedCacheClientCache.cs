// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

internal class DistributedCacheClientCache
{
    public static ConcurrentDictionary<string, IManualDistributedCacheClient> CacheClients { get; set; } = new();

    public IManualDistributedCacheClient GetCacheClient(IServiceProvider serviceProvider)
    {
        var multiEnvironmentContext = serviceProvider.GetRequiredService<IMultiEnvironmentContext>();
        var environment = multiEnvironmentContext.CurrentEnvironment;

        return CacheClients.GetOrAdd(environment, env =>
        {
            var cacheClient = serviceProvider.GetRequiredService<ScopedService<IManualDistributedCacheClient>>().Service;
            return cacheClient;
        });
    }
}
