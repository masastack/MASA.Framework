// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public class MultilevelCacheOptions : CacheOptions
{
    /// <summary>
    /// Memory default valid time configuration
    /// When the memory cache does not exist, the result obtained from the distributed cache is used when the result is newly written to the memory cache.
    /// If not specified, the global configuration is used by default
    /// </summary>
    public CacheEntryOptions? MemoryCacheEntryOptions { get; set; }
}
