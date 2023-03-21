// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.MultilevelCache;

internal class DefaultMultilevelCachePool : IMultilevelCachePool
{
    private readonly MemoryCache<string, (IMemoryCache MemoryCache, IManualDistributedCacheClient DistributedCacheClient)> _data = new();

    public (IMemoryCache MemoryCache, IManualDistributedCacheClient ManualDistributedCacheClient) GetCache(
        IServiceProvider serviceProvider,
        string name,
        MultilevelCacheGlobalOptions multilevelCacheGlobalOptions)
        => _data.GetOrAdd(ConvertToKey(name, multilevelCacheGlobalOptions), _ =>
        {
            var memoryCache = new MemoryCache(multilevelCacheGlobalOptions);
            var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(name);
            return (memoryCache, distributedCacheClient);
        });

    public void TryRemove(string name, MultilevelCacheGlobalOptions multilevelCacheGlobalOptions)
    {
        string key = ConvertToKey(name, multilevelCacheGlobalOptions);
        if (_data.TryGet(key, out var item))
        {
            _data.Remove(key);
            item.MemoryCache.Dispose();
            item.DistributedCacheClient.Dispose();
        }
    }

    private static string ConvertToKey(string name, MultilevelCacheGlobalOptions multilevelCacheGlobalOptions)
        => $"{name}{multilevelCacheGlobalOptions.InstanceId ?? string.Empty}";

    public void Dispose()
    {
        foreach (var item in _data.Values)
        {
            item.MemoryCache.Dispose();
            item.DistributedCacheClient.Dispose();
        }

        _data.Dispose();
    }
}
