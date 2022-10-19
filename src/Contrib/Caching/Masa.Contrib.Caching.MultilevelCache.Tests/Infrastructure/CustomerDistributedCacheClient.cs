// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests.Infrastructure;

public class CustomerDistributedCacheClient : MultilevelCacheClient
{
    public CustomerDistributedCacheClient(CacheEntryOptions? cacheEntryOptions)
        : base(cacheEntryOptions)
    {
    }

    public MemoryCacheEntryOptions? GetBaseMemoryCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
        => base.GetMemoryCacheEntryOptions(cacheEntryOptions);
}
