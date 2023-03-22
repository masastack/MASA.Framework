// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisValueExtensions
{
    public static HashEntry[] ConvertToHashEntries(
        this RedisValue redisValue,
        CacheExpiredModel cacheExpiredModel)
    {
        var hashEntries = new List<HashEntry>()
        {
            new(RedisConstant.ABSOLUTE_EXPIRATION_KEY, cacheExpiredModel.AbsoluteExpirationTicks),
            new(RedisConstant.SLIDING_EXPIRATION_KEY, cacheExpiredModel.SlidingExpirationTicks),
            new HashEntry(RedisConstant.DATA_KEY, redisValue)
        };

        return hashEntries.ToArray();
    }
}
