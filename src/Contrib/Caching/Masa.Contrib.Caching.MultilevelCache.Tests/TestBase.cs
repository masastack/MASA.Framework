// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests;

public class TestBase
{
    protected const string REDIS_HOST = "localhost";

    protected static RedisConfigurationOptions RedisConfigurationOptions
        => new()
        {
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST, 6379)
            },
            GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            }
        };

    public static void CheckLifeCycle(IDatabase database, string key, double minVal, double maxVal)
    {
        var expireTimeSpan = database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds <= maxVal && expireTimeSpan.Value.TotalSeconds >= minVal);
    }
}
