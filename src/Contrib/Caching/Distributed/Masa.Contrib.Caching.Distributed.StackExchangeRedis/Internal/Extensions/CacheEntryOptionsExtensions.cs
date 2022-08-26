// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class CacheEntryOptionsExtensions
{
    internal static DateTimeOffset? GetAbsoluteExpiration(this CacheEntryOptions options, DateTimeOffset creationTime)
    {
        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            throw new ArgumentOutOfRangeException(
                nameof(CacheEntryOptions.AbsoluteExpiration),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            return creationTime.Add(options.AbsoluteExpirationRelativeToNow.Value);

        return options.AbsoluteExpiration;
    }
}
