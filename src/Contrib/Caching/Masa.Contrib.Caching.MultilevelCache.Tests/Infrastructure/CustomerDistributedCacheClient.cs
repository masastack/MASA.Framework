// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests.Infrastructure;

public class CustomerDistributedCacheClient : MultilevelCacheClient
{
    public CustomerDistributedCacheClient(IMemoryCache memoryCache,
        IDistributedCacheClient distributedCacheClient,
        MultilevelCacheOptions multilevelCacheOptions,
        SubscribeKeyType subscribeKeyType,
        string subscribeKeyPrefix = "",
        ITypeAliasProvider? typeAliasProvider = null)
        : base(memoryCache, distributedCacheClient, multilevelCacheOptions, subscribeKeyType, subscribeKeyPrefix, typeAliasProvider)
    {
    }

    public MemoryCacheEntryOptions? GetBaseMemoryCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
        => base.GetMemoryCacheEntryOptions(cacheEntryOptions);
}
