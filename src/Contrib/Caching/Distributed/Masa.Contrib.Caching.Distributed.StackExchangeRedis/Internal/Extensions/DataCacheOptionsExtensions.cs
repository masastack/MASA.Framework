// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class DataCacheOptionsExtensions
{
    internal static (bool State, TimeSpan? Expire) GetExpiration(
        this DataCacheBaseModel model,
        DateTimeOffset? createTime = null,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (model.SlidingExpiration.HasValue)
        {
            TimeSpan? expr;
            if (model.AbsoluteExpiration.HasValue)
            {
                var sldExpr = GetSlidingExpiration(model.SlidingExpiration);
                var absExpr = new DateTimeOffset(model.AbsoluteExpiration.Value, TimeSpan.Zero);

                var relExpr = absExpr - (createTime ?? DateTimeOffset.Now);
                expr = relExpr <= sldExpr ? relExpr : sldExpr;
            }
            else
            {
                expr = GetSlidingExpiration(model.SlidingExpiration);
            }

            return (true, expr);
        }

        return (false, null);
    }

    private static TimeSpan GetSlidingExpiration(long? slidingExpiration) => new(slidingExpiration!.Value);
}
