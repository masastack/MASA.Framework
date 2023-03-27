// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.MultilevelCache;

/// <summary>
/// The MASA Multilevel cache options.
/// </summary>
public class MultilevelCacheGlobalOptions : MemoryCacheOptions
{
    public CacheOptions GlobalCacheOptions { get; set; } = new()
    {
        CacheKeyType = CacheKeyType.TypeName
    };

    /// <summary>
    /// Gets or sets the <see cref="SubscribeKeyType"/>.
    /// </summary>
    public SubscribeKeyType SubscribeKeyType { get; set; } = SubscribeKeyType.ValueTypeFullNameAndKey;

    /// <summary>
    /// Gets or sets the prefix of subscribe key.
    /// </summary>
    public string SubscribeKeyPrefix { get; set; } = string.Empty;

    public string? InstanceId { get; set; }

    /// <summary>
    /// Memory default valid time configuration
    /// </summary>
    public CacheEntryOptions? CacheEntryOptions { get; set; }
}
