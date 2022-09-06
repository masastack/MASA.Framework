// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class StackExchangeRedisCacheTest : TestBase
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
                new(REDIS_HOST)
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
                new(REDIS_HOST)
            }
        });
        services.AddStackExchangeRedisCache("test2", new RedisConfigurationOptions()
        {
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
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

    [TestMethod]
    public void TestAddStackExchangeRedisCacheByAppsettings()
    {
        var builder = WebApplication.CreateBuilder();
        var rootPath = builder.Environment.ContentRootPath;
        var services = builder.Services;
        services.AddStackExchangeRedisCache("test");

        var serviceProvider = services.BuildServiceProvider();
        var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClient>();
        string key = "test_1";
        distributedCacheClient.Set(key, "test_content");
        Assert.IsTrue(distributedCacheClient.Exists(key));

        var oldContent = File.ReadAllText(Path.Combine(rootPath, "appsettings.json"));
        File.WriteAllText(Path.Combine(rootPath, "appsettings.json"),
            JsonSerializer.Serialize(new
            {
                RedisConfig = new RedisConfigurationOptions()
                {
                    Servers = new List<RedisServerOptions>()
                    {
                        new(REDIS_HOST, 6379)
                    },
                    DefaultDatabase = 1
                }
            }));

        Thread.Sleep(3000);
        distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create();

        var exist = distributedCacheClient.Exists(key);

        Assert.IsFalse(exist);

        File.WriteAllText(Path.Combine(Path.Combine(rootPath, "appsettings.json")), oldContent);

        Thread.Sleep(3000);

        distributedCacheClient.Remove(key);
    }

    [TestMethod]
    public void TestAddStackExchangeRedisCacheRepeat()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache(new RedisConfigurationOptions()
        {
            DefaultDatabase = 1,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            }
        });
        services.AddStackExchangeRedisCache(new RedisConfigurationOptions()
        {
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            }
        });
        var serviceProvider = services.BuildServiceProvider();

        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        Assert.IsNotNull(distributedCacheClient);

        Assert.IsTrue(distributedCacheClient is DistributedCacheClient redisClient);
        var fieldInfo = typeof(DistributedCacheClient).GetField("Db", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(fieldInfo);

        var value = fieldInfo.GetValue((DistributedCacheClient)distributedCacheClient);
        Assert.IsNotNull(value);
        Assert.AreEqual(1, ((IDatabase)value).Database);
    }
}
