// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class RefreshCacheOptions
{
    public string Key { get; }

    public DateTimeOffset? AbsExpr { get; }

    public TimeSpan? SldExpr { get; }

    public RefreshCacheOptions(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr)
    {
        Key = key;
        AbsExpr = absExpr;
        SldExpr = sldExpr;
    }
}
