﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class TimeHelper
{
    public static long? GetExpirationInSeconds(
        DateTimeOffset creationTime,
        DateTimeOffset? absoluteExpiration,
        TimeSpan? slidingExpiration)
    {
        if (absoluteExpiration.HasValue && slidingExpiration.HasValue)
            return (long)Math.Min(
                (absoluteExpiration.Value - creationTime).TotalSeconds,
                slidingExpiration.Value.TotalSeconds);

        if (absoluteExpiration.HasValue)
            return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;

        if (slidingExpiration.HasValue)
            return (long)slidingExpiration.Value.TotalSeconds;

        return null;
    }
}
