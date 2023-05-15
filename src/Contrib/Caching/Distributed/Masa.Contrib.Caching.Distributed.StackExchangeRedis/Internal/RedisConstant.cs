// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisConstant
{
    public const string DEFAULT_REDIS_SECTION_NAME = "RedisConfig";

    public const string DEFAULT_REDIS_HOST = "localhost";

    public const int DEFAULT_REDIS_PORT = 6379;

    public const string ABSOLUTE_EXPIRATION_KEY = "absexp";

    public const string SLIDING_EXPIRATION_KEY = "sldexp";

    public const string DATA_KEY = "data";

    public const int DEADLINE_LASTING = -1;

    public const string SET_MULTIPLE_SCRIPT = @"
                local count = 0
                for i, key in ipairs(KEYS) do
                  redis.call('HSET', key, '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2], '" +
        DATA_KEY + @"', ARGV[i+3])
                  if ARGV[3] ~= '-1' then
                    redis.call('EXPIRE', key, ARGV[3])
                  end
                  count = count + 1
                end
                return count";

    public const string SET_EXPIRE_SCRIPT = @"
        for index,key in ipairs(KEYS) do redis.call('expire', key, ARGV[index]) end;
        return 1";

    public const string GET_KEYS_SCRIPT = @"return redis.call('keys', @pattern)";

    public const string GET_KEY_AND_VALUE_SCRIPT = @"local ks = redis.call('KEYS', @keypattern)
        local result = {}
        for index,val in pairs(ks) do result[(2 * index - 1)] = val; result[(2 * index)] = redis.call('hgetall', val) end;
        return result";

    // KEYS[1] = key
    // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
    // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
    // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
    // this order should not change LUA script depends on it
    public const string SET_EXPIRATION_SCRIPT = @"
                local exist = redis.call('EXISTS',KEYS[1])
                if(exist ~= 1) then
                  return 0 end
                redis.call('HSET', KEYS[1], '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                else
                  redis.call('PERSIST', KEYS[1])
                end
                return 1";

    public const string SET_MULTIPLE_EXPIRATION_SCRIPT = @"
                local count = 0
                for i, key in ipairs(KEYS) do
                  if(redis.call('EXISTS', key) == 1) then
                    redis.call('HSET', key, '" + ABSOLUTE_EXPIRATION_KEY + "', ARGV[1], '" + SLIDING_EXPIRATION_KEY + @"', ARGV[2])
                    if ARGV[3] ~= '-1' then
                      redis.call('EXPIRE', key, ARGV[3])
                    else
                      redis.call('PERSIST', key)
                    end
                    count = count + 1
                  end
                end
                return count";
}
