// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class CombinedCacheEntry<T>
{
    public CacheEntryOptions? MemoryCacheEntryOptions { get; set; }

    public Func<CacheEntry<T>> DistributedCacheEntryFunc { get; set; }
}
