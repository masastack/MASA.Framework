// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Data stored to Redis
/// </summary>
internal class DataCacheOptions : RefreshCacheOptions
{
    public RedisValue Value { get; }

    public DataCacheOptions(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, RedisValue value)
        : base(key, absExpr, sldExpr)
    {
        Value = value;
    }
}
