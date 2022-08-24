// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RefreshCacheOptionsExtensions
{
    internal static bool RefreshCore(
        this CacheEntryOptions options,
        string key,
        Func<string, TimeSpan?, Task<bool>> func,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        // Note Refresh has no effect if there is just an absolute expiration (or neither).
        if (options.SlidingExpiration.HasValue)
        {
            TimeSpan? expr;
            if (options.AbsoluteExpiration.HasValue)
            {
                var relExpr = options.AbsoluteExpiration.Value - DateTimeOffset.Now;
                expr = relExpr <= options.SlidingExpiration.Value ? relExpr : options.SlidingExpiration;
            }
            else
            {
                expr = options.SlidingExpiration;
            }

            return func.Invoke(key, expr).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        return false;
    }

    internal static DateTimeOffset? GetAbsoluteExpiration(this CacheEntryOptions? options, DateTimeOffset? creationTime)
    {
        if (options == null)
            return null;

        creationTime ??= DateTimeOffset.UtcNow;

        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            throw new ArgumentOutOfRangeException(
                nameof(CacheEntryOptions.AbsoluteExpiration),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            return creationTime.Value.Add(options.AbsoluteExpirationRelativeToNow.Value);

        return options.AbsoluteExpiration;
    }
}
