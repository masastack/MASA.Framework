// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public class MultilevelCachePublish<T>
{
    public T? Value { get; set; }

    public CacheEntryOptions? CacheEntryOptions { get; set; }

    public MultilevelCachePublish()
    {

    }

    public MultilevelCachePublish(T? value, CacheEntryOptions? cacheEntryOptions = null) : this()
    {
        Value = value;
        CacheEntryOptions = cacheEntryOptions;
    }
}
