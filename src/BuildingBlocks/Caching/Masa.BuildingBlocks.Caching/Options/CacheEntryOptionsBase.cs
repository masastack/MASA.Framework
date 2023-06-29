// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public abstract class CacheEntryOptionsBase
{
    #region Set the expiration time to ensure that the writing method remains unchanged

    /// <summary>
    /// Gets or sets an absolute expiration date for the cache entry.
    /// When coexisting with AbsoluteExpirationRelativeToNow, use AbsoluteExpirationRelativeToNow first
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration
    {
        get => CacheOptions?.AbsoluteExpiration;
        set
        {
            CacheOptions ??= new CacheEntryOptions();
            CacheOptions.AbsoluteExpiration = value;
        }
    }

    /// <summary>
    /// Gets or sets an absolute expiration time, relative to now.
    /// When coexisting with AbsoluteExpiration, use AbsoluteExpirationRelativeToNow first
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow
    {
        get => CacheOptions?.AbsoluteExpirationRelativeToNow;
        set
        {
            CacheOptions ??= new CacheEntryOptions();
            CacheOptions.AbsoluteExpirationRelativeToNow = value;
        }
    }

    /// <summary>
    /// Gets or sets how long a cache entry can be inactive (e.g. not accessed) before it will be removed.
    /// This will not extend the entry lifetime beyond the absolute expiration (if set).
    /// </summary>
    public TimeSpan? SlidingExpiration
    {
        get => CacheOptions?.SlidingExpiration;
        set
        {
            CacheOptions ??= new CacheEntryOptions();
            CacheOptions.SlidingExpiration = value;
        }
    }

    #endregion

    public CacheEntryOptions? CacheOptions { get; set; }

    protected CacheEntryOptionsBase()
    {
        CacheOptions = null;
    }

    protected CacheEntryOptionsBase(DateTimeOffset? absoluteExpiration) : this()
    {
        AbsoluteExpiration = absoluteExpiration;
    }

    protected CacheEntryOptionsBase(TimeSpan? absoluteExpirationRelativeToNow) : this()
    {
        AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
    }
}
