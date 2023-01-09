// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class CombinedCacheEntry<T>
{
    public CacheEntryOptions? MemoryCacheEntryOptions { get; set; }

    public Func<CacheEntry<T>>? DistributedCacheEntryFunc { get; set; }

    /// <summary>
    /// Only async methods are supported
    /// </summary>
    public Func<Task<CacheEntry<T>>>? DistributedCacheEntryAsyncFunc { get; set; }

    public CombinedCacheEntry()
    {
    }

    private CombinedCacheEntry(CacheEntryOptions? memoryCacheEntryOptions) : this()
    {
        MemoryCacheEntryOptions = memoryCacheEntryOptions;
    }

    public CombinedCacheEntry(Func<CacheEntry<T>> distributedCacheEntryFunc, CacheEntryOptions? memoryCacheEntryOptions)
        : this(memoryCacheEntryOptions)
    {
        DistributedCacheEntryFunc = distributedCacheEntryFunc;
    }

    public CombinedCacheEntry(Func<Task<CacheEntry<T>>> distributedCacheEntryAsyncFunc, CacheEntryOptions? memoryCacheEntryOptions)
        : this(memoryCacheEntryOptions)
    {
        DistributedCacheEntryAsyncFunc = distributedCacheEntryAsyncFunc;
    }
}
