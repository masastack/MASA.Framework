// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

public class TestBase
{
    protected RedisConfigurationOptions GetConfigurationOptions()
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        redisConfigurationOptions.Servers.Add(new RedisServerOptions());
        return redisConfigurationOptions;
    }

    protected CacheEntryOptions GetCacheEntryOptions()
    {
        var cacheEntryOptions = new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };
        return cacheEntryOptions;
    }

    protected JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
        jsonSerializerOptions.EnableDynamicTypes();
        return jsonSerializerOptions;
    }
}
