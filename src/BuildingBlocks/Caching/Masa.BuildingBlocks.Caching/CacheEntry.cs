// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class CacheEntry<T> : CacheEntryOptionsBase
{
    public T? Value { get; }

    public CacheEntry(T? value)
    {
        Value = value;
    }

    public CacheEntry(T? value, DateTimeOffset absoluteExpiration) : this(value)
        => AbsoluteExpiration = absoluteExpiration;

    public CacheEntry(T? value, TimeSpan absoluteExpirationRelativeToNow) : this(value)
        => AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
}
