// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class DataCacheOptionsExtensions
{
    internal static (bool State, TimeSpan? Expire) GetExpiration(
        this DataCacheOptions options,
        DateTimeOffset? createTime = null,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (options.SlidingExpiration.HasValue)
        {
            TimeSpan? expr;
            if (options.AbsoluteExpiration.HasValue)
            {
                var sldExpr = GetSlidingExpiration(options.SlidingExpiration);
                var absExpr = new DateTimeOffset(options.AbsoluteExpiration.Value, TimeSpan.Zero);

                var relExpr = absExpr - (createTime ?? DateTimeOffset.Now);
                expr = relExpr <= sldExpr ? relExpr : sldExpr;
            }
            else
            {
                expr = GetSlidingExpiration(options.SlidingExpiration);
            }

            return (true, expr);
        }

        return (false, null);
    }

    private static TimeSpan GetSlidingExpiration(long? slidingExpiration) => new(slidingExpiration!.Value);
}
