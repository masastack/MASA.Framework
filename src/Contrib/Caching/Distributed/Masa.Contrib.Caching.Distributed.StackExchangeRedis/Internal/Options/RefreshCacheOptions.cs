// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal class RefreshCacheOptions
{
    public string Key { get; }

    public DateTimeOffset? AbsExpr { get; }

    public TimeSpan? SldExpr { get; }

    public RedisValue Value { get; }

    public RefreshCacheOptions(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, RedisValue value)
    {
        Key = key;
        AbsExpr = absExpr;
        SldExpr = sldExpr;
        Value = value;
    }
}
