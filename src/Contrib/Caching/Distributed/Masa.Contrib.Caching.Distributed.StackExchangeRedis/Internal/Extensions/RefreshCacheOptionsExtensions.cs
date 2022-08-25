// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RefreshCacheOptionsExtensions
{
    internal static (bool State, TimeSpan? Expire) Refresh(
        this RefreshCacheOptions options,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (options.SldExpr.HasValue)
        {
            TimeSpan? expr;
            if (options.AbsExpr.HasValue)
            {
                var relExpr = options.AbsExpr.Value - DateTimeOffset.Now;
                expr = relExpr <= options.SldExpr.Value ? relExpr : options.SldExpr;
            }
            else
            {
                expr = options.SldExpr;
            }

            return (true, expr);
        }

        return (false, null);
    }
}
