// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Data stored to Redis
/// </summary>
internal class DataCacheOptions
{
    public string Key { get; }

    public long? AbsoluteExpiration { get; }

    public long? SlidingExpiration { get; }

    public RedisValue Value { get; }

    public DataCacheOptions(string key, long? absoluteExpiration, long? slidingExpiration, RedisValue value)
    {
        Key = key;
        AbsoluteExpiration = absoluteExpiration;
        SlidingExpiration = slidingExpiration;
        Value = value;
    }
}
