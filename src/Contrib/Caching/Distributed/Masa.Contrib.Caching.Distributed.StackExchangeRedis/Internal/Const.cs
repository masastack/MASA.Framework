// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal sealed class Const
{
    public const string DEFAULT_REDIS_HOST = "localhost";

    public const int DEFAULT_REDIS_PORT = 6379;

    public const string ABSOLUTE_EXPIRATION_KEY = "absexp";

    public const string SLIDING_EXPIRATION_KEY = "sldexp";

    public const string DATA_KEY = "data";

    public const int DEADLINE_LASTING = -1;

    // Reference from https://github.com/dotnet/aspnetcore/blob/3c793666742cfc4c389292f3378d15e32f860dc9/src/Caching/StackExchangeRedis/src/RedisCache.cs#L372
    // KEYS[1] = = key
    // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
    // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
    // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
    // ARGV[4] = data - byte[]
    // this order should not change LUA script depends on it
    public const string SET_SCRIPT = @"
                redis.call('HSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1";

    public const string GET_LIST_SCRIPT = @"local result = {}
        for index,val in pairs(@keys) do result[(2 * index - 1)] = val; result[(2 * index)] = redis.call('hgetall', val) end;
        return result";

    public const string GET_KEYS_SCRIPT = @"return redis.call('keys', @pattern)";

    public const string GET_KEY_AND_VALUE_SCRIPT = @"local ks = redis.call('KEYS', @keypattern)
        local result = {}
        for index,val in pairs(ks) do result[(2 * index - 1)] = val; result[(2 * index)] = redis.call('hgetall', val) end;
        return result";
}
