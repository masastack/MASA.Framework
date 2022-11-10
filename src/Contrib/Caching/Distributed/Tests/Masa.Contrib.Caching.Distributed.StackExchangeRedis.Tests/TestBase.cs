// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

public class TestBase
{
    protected const string REDIS_HOST = "localhost";

    protected static RedisConfigurationOptions GetConfigurationOptions()
    {
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            }
        };
        redisConfigurationOptions.Servers.Add(new RedisServerOptions());
        return redisConfigurationOptions;
    }

    protected static CacheEntryOptions GetCacheEntryOptions()
    {
        var cacheEntryOptions = new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };
        return cacheEntryOptions;
    }

    protected static JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        return jsonSerializerOptions;
    }
}
