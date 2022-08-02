using StackExchange.Redis;

// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis.Extensions;

/// <summary>
/// Reference from https://github.com/dotnet/aspnetcore/blob/3c79366674/src/Caching/StackExchangeRedis/src/RedisExtensions.cs
/// </summary>
internal static class RedisExtensions
{
    internal static RedisValue[] HashMemberGet(this IDatabase cache, string key, params string[] members)
    {
        // TODO: Error checking?
        return cache.HashGet(key, GetRedisMembers(members));
    }

    internal static async Task<RedisValue[]> HashMemberGetAsync(
        this IDatabase cache,
        string key,
        params string[] members)
    {
        // TODO: Error checking?
        return await cache.HashGetAsync(key, GetRedisMembers(members)).ConfigureAwait(false);
    }

    private static RedisValue[] GetRedisMembers(params string[] members)
    {
        var redisMembers = new RedisValue[members.Length];
        for (int i = 0; i < members.Length; i++)
        {
            redisMembers[i] = members[i];
        }

        return redisMembers;
    }
}
