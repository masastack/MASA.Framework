// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

public class TestBase
{
    protected const string REDIS_HOST = "localhost";

    protected static readonly int DatabaseIndex = Environment.Version.Major;

    protected static RedisConfigurationOptions GetConfigurationOptions()
    {
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            // Each target framework uses its own Redis database, so test processes running concurrently (net8.0 / net9.0 ...)
            // do not read, overwrite or delete each other's keys
            DefaultDatabase = DatabaseIndex,
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

    public static void CheckLifeCycle(IDatabase database, string key, double minVal, double maxVal)
    {
        var expireTimeSpan = database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds <= maxVal && expireTimeSpan.Value.TotalSeconds >= minVal);
    }
}
