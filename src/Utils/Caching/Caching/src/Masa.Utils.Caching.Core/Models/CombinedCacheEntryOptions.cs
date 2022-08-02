// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.Models;

/// <summary>
/// The combined cache entry options.
/// </summary>
public class CombinedCacheEntryOptions
{
    /// <summary>
    /// Gets or sets the memory cache entry options.
    /// </summary>
    public MemoryCacheEntryOptions? MemoryCacheEntryOptions { get; set; }

    /// <summary>
    /// Gets or sets the distributed cache entry options.
    /// </summary>
    public DistributedCacheEntryOptions? DistributedCacheEntryOptions { get; set; }

    public CombinedCacheEntryOptions()
    {
    }

    public CombinedCacheEntryOptions(MemoryCacheEntryOptions? memoryCacheEntryOptions, DistributedCacheEntryOptions? distributedCacheEntryOptions)
    {
        MemoryCacheEntryOptions = memoryCacheEntryOptions;
        DistributedCacheEntryOptions = distributedCacheEntryOptions;
    }
}

/// <summary>
/// The combined cache entry options.
/// </summary>
public class CombinedCacheEntryOptions<T>
{
    public Action<T?>? ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the memory cache entry options.
    /// </summary>
    public MemoryCacheEntryOptions? MemoryCacheEntryOptions { get; set; }

    /// <summary>
    /// Gets or sets the distributed cache entry options.
    /// </summary>
    public DistributedCacheEntryOptions? DistributedCacheEntryOptions { get; set; }

    /// <summary>
    /// Determines whether to ignore subscribe.
    /// </summary>
    public bool IgnoreSubscribe { get; set; }

    public CombinedCacheEntryOptions Standard => new(MemoryCacheEntryOptions, DistributedCacheEntryOptions);
}
