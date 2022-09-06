// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public abstract class BaseDistributedCacheClient : AbstractMultilevelCacheClient
{
    public CacheEntryOptions? DefaultCacheEntryOptions { get; protected set; }

    protected BaseDistributedCacheClient(CacheEntryOptions? cacheEntryOptions)
    {
        DefaultCacheEntryOptions = cacheEntryOptions;
    }

    protected static string FormatMemoryCacheKey<T>(string key) => SubscribeHelper.FormatMemoryCacheKey<T>(key);

    protected MemoryCacheEntryOptions? GetMemoryCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
    {
        var options = cacheEntryOptions ?? DefaultCacheEntryOptions;
        if (options == null)
            return null;

        return CopyTo(options);
    }

    private static MemoryCacheEntryOptions CopyTo(CacheEntryOptions cacheEntryOptions)
    {
        return new()
        {
            AbsoluteExpiration = cacheEntryOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = cacheEntryOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = cacheEntryOptions.SlidingExpiration
        };
    }
}
