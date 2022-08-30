// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class StackExchangeRedisCacheTest
{
    [TestMethod]
    public void TestAddStackExchangeRedisCache()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache(new RedisConfigurationOptions()
        {
            DefaultDatabase = 1,
            Servers = new List<RedisServerOptions>()
            {
                new("localhost")
            }
        });
        var serviceProvider = services.BuildServiceProvider();

        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        Assert.IsNotNull(distributedCacheClient);
        string key = "test_key";
        distributedCacheClient.Set(key, "content");
        Assert.IsTrue(distributedCacheClient.Exists(key));
        distributedCacheClient.Remove(key);
    }

    [TestMethod]
    public void TestAddMultiStackExchangeRedisCache()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache("test", new RedisConfigurationOptions()
        {
            DefaultDatabase = 1,
            Servers = new List<RedisServerOptions>()
            {
                new("localhost")
            }
        }).AddStackExchangeRedisCache("test2", new RedisConfigurationOptions()
        {
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new("localhost")
            }
        });
        var serviceProvider = services.BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
        var distributedCacheClient = factory.Create("test");
        Assert.IsNotNull(distributedCacheClient);
        string key = "test_key";
        distributedCacheClient.Set(key, "content");
        Assert.IsTrue(distributedCacheClient.Exists(key));

        var distributedCacheClient2 = factory.Create("test2");
        Assert.IsFalse(distributedCacheClient2.Exists(key));
        distributedCacheClient2.Set(key + "2", "content_2");

        Assert.AreEqual("content_2", distributedCacheClient2.Get<string>(key + "2"));
        Assert.AreEqual(null, distributedCacheClient.Get<string>(key + "2"));
        distributedCacheClient.Remove(key);
        distributedCacheClient2.Remove(key + "2");
    }
}
