// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class CombinedCacheEntry<T>
{
    /// <summary>
    /// Memory cache lifetime configuration
    /// When the memory cache expiration time is not set, the memory cache expiration time is consistent with the configuration when registering the multi-level cache
    /// </summary>
    public Action<CacheEntryOptions>? MemoryCacheEntryOptionsAction { get; set; }

    public Func<CacheEntry<T>>? DistributedCacheEntryFunc { get; set; }

    /// <summary>
    /// Only async methods are supported
    /// When both DistributedCacheEntryFunc and DistributedCacheEntryAsyncFunc are set, DistributedCacheEntryFunc is preferred
    /// </summary>
    public Func<Task<CacheEntry<T>>>? DistributedCacheEntryAsyncFunc { get; set; }

    public CombinedCacheEntry()
    {
    }

    private CombinedCacheEntry(Action<CacheEntryOptions>? memoryCacheEntryOptionsAction) : this()
    {
        MemoryCacheEntryOptionsAction = memoryCacheEntryOptionsAction;
    }

    public CombinedCacheEntry(Func<CacheEntry<T>> distributedCacheEntryFunc, Action<CacheEntryOptions>? memoryCacheEntryOptionsAction)
        : this(memoryCacheEntryOptionsAction)
    {
        DistributedCacheEntryFunc = distributedCacheEntryFunc;
    }

    public CombinedCacheEntry(Func<Task<CacheEntry<T>>> distributedCacheEntryAsyncFunc, Action<CacheEntryOptions>? memoryCacheEntryOptionsAction)
        : this(memoryCacheEntryOptionsAction)
    {
        DistributedCacheEntryAsyncFunc = distributedCacheEntryAsyncFunc;
    }
}
