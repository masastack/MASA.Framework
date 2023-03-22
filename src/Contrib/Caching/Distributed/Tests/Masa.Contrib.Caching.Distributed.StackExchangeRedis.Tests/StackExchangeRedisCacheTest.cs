// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Net;

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

#pragma warning disable CS0618
[TestClass]
public class StackExchangeRedisCacheTest : TestBase
{
    private static readonly PropertyInfo DataBasePropertyInfo =
        typeof(RedisCacheClient).GetProperty("Db", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [TestMethod]
    public void TestAddStackExchangeRedisCache()
    {
        var services = new ServiceCollection();
        services.AddDistributedCache(distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(option =>
        {
            option.DefaultDatabase = 1;
            option.Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            };
            option.GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            };
        }));

        var serviceProvider = services.BuildServiceProvider();

        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        Assert.IsNotNull(distributedCacheClient);
        string key = "test_key";
        distributedCacheClient.Set(key, "content");
        Assert.IsTrue(distributedCacheClient.Exists(key));
        distributedCacheClient.Remove(key);

        var database = GetDatabase(distributedCacheClient as RedisCacheClient);
        Assert.AreEqual(1, database.Database);
    }

    private static IDatabase GetDatabase(RedisCacheClient? redisCacheClient)
    {
        Assert.IsNotNull(redisCacheClient);
        var database = DataBasePropertyInfo.GetValue(redisCacheClient) as IDatabase;
        Assert.IsNotNull(database);
        return database;
    }

    [TestMethod]
    public void TestAddMultiStackExchangeRedisCache()
    {
        var services = new ServiceCollection();
        services.AddDistributedCache("test", builder => builder.UseStackExchangeRedisCache(option =>
        {
            option.DefaultDatabase = 1;
            option.Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            };
            option.GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            };
        }));
        services.AddDistributedCache("test2", builder => builder.UseStackExchangeRedisCache(new RedisConfigurationOptions()
        {
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            },
            GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            }
        }));
        var serviceProvider = services.BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
        using var distributedCacheClient = factory.Create("test");
        Assert.IsNotNull(distributedCacheClient);
        string key = "test_key";
        distributedCacheClient.Set(key, "content");
        Assert.IsTrue(distributedCacheClient.Exists(key));

        using var distributedCacheClient2 = factory.Create("test2");
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
        services.AddDistributedCache("test", distributeCacheBuilder => distributeCacheBuilder.UseStackExchangeRedisCache());

        var serviceProvider = services.BuildServiceProvider();
        var distributedCacheClient = serviceProvider.GetRequiredService<IManualDistributedCacheClient>();
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

        Task.Delay(3000).ConfigureAwait(false).GetAwaiter().GetResult();
        using (distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create())
        {
            var exist = distributedCacheClient.Exists(key);

            Assert.IsTrue(exist);
        }

        using (distributedCacheClient =
                   serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create())
        {
            var exist = distributedCacheClient.Exists(key);

            Assert.IsFalse(exist);

            File.WriteAllText(Path.Combine(Path.Combine(rootPath, "appsettings.json")), oldContent);

            Task.Delay(3000).ConfigureAwait(false).GetAwaiter().GetResult();

            distributedCacheClient.Remove(key);
        }
    }

    [TestMethod]
    public void TestAddStackExchangeRedisCacheRepeat()
    {
        var services = new ServiceCollection();
        services.AddDistributedCache(distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache(options =>
        {
            options.DefaultDatabase = 1;
            options.Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            };
            options.GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            };
        }));
        services.AddDistributedCache(distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache(
            new RedisConfigurationOptions()
            {
                DefaultDatabase = 2,
                Servers = new List<RedisServerOptions>()
                {
                    new(REDIS_HOST)
                },
                GlobalCacheOptions = new CacheOptions()
                {
                    CacheKeyType = CacheKeyType.None
                }
            }));
        var serviceProvider = services.BuildServiceProvider();

        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        Assert.IsNotNull(distributedCacheClient);

        var value = GetDatabase(distributedCacheClient as RedisCacheClient);
        Assert.IsNotNull(value);
        Assert.AreEqual(1, ((IDatabase)value).Database);
    }

    [TestMethod]
    public void TestAddStackExchangeRedisCacheRepeatByConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDistributedCache(distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache());
        builder.Services.AddDistributedCache(Options.DefaultName,
            distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache("RedisConfig2"));
        builder.Services.AddDistributedCache(Options.DefaultName,
            distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache("RedisConfig3"));

        var serviceProvider = builder.Services.BuildServiceProvider();

        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        Assert.IsNotNull(distributedCacheClient);

        var value = GetDatabase(distributedCacheClient as RedisCacheClient);
        Assert.IsNotNull(value);
        Assert.AreEqual(6, ((IDatabase)value).Database);
    }

    [TestMethod]
    public void TestFormatCacheKey()
    {
        var services = new ServiceCollection();
        services.AddDistributedCache("test", distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(options =>
        {
            options.Servers = new List<RedisServerOptions>()
            {
                new()
            };
            options.GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.TypeName
            };
        }));

        services.AddDistributedCache("test2", distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(options =>
        {
            options.Servers = new List<RedisServerOptions>()
            {
                new(),
            };
            options.DefaultDatabase = 0;
            options.GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            };
        }));
        var serviceProvider = services.BuildServiceProvider();
        var distributedCacheClientFactory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
        var key = "redisConfig";
        using var distributedCacheClient = distributedCacheClientFactory.Create("test");
        Assert.IsNotNull(distributedCacheClient);
        distributedCacheClient.Remove<string>(key);
        distributedCacheClient.Set(key, "redis configuration json");
        var value = distributedCacheClient.Get<string>(key);
        Assert.AreEqual("redis configuration json", value);

        using var distributedCacheClient2 = distributedCacheClientFactory.Create("test2");
        Assert.IsNotNull(distributedCacheClient2);

        Assert.IsFalse(distributedCacheClient2.Exists(key));
        Assert.IsFalse(distributedCacheClient2.Exists<string>(key));

        distributedCacheClient2.Set(key, "redis configuration2 json");
        var value2 = distributedCacheClient2.Get<string>(key);
        Assert.AreEqual("redis configuration2 json", value2);

        distributedCacheClient.Remove<string>(key);
        distributedCacheClient2.Remove<string>(key);
    }

    [TestMethod]
    public void TestFormatCacheKeyByTypeNameAlias()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDistributedCache("test", distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache("RedisConfig4");
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        using var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create("test");
        Assert.IsNotNull(distributedCacheClient);

        Assert.ThrowsException<NotImplementedException>(() =>
        {
            distributedCacheClient.GetOrSet("redisConfiguration", () => new CacheEntry<string>("redis configuration2 json"));
        });
    }

    [TestMethod]
    public void TestFormatCacheKeyByTypeNameAlias2()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDistributedCache("test", distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache("RedisConfig4");
        });
        builder.Services.Configure("test", (TypeAliasOptions options) =>
        {
            options.GetAllTypeAliasFunc = () => new Dictionary<string, string>()
            {
                {
                    "String", "s"
                }
            };
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        using var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create("test");
        Assert.IsNotNull(distributedCacheClient);

        var value = distributedCacheClient.GetOrSet("redisConfiguration", () => new CacheEntry<string>("redis configuration2 json"));
        Assert.AreEqual("redis configuration2 json", value);
        distributedCacheClient.Remove<string>("redisConfiguration");
    }

    [TestMethod]
    public void TestFormatCacheKeyByTypeNameAlias3()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDistributedCache(
            distributedCacheOptions =>
            {
                distributedCacheOptions.UseStackExchangeRedisCache("RedisConfig4");
            },
            typeAliasOptions =>
            {
                typeAliasOptions.GetAllTypeAliasFunc = () => new Dictionary<string, string>()
                {
                    {
                        "String", "s"
                    }
                };
            });
        builder.Services.Configure((TypeAliasOptions options) =>
        {
            options.GetAllTypeAliasFunc = () => new Dictionary<string, string>()
            {
                {
                    "String", "s"
                }
            };
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        using var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create();
        Assert.IsNotNull(distributedCacheClient);

        var value = distributedCacheClient.GetOrSet("redisConfiguration", () => new CacheEntry<string>("redis configuration2 json"));
        Assert.AreEqual("redis configuration2 json", value);
        distributedCacheClient.Remove<string>("redisConfiguration");
    }

    [TestMethod]
    public async Task TestAsync()
    {
        var redis = new ConfigurationOptions();
        redis.EndPoints.Add(new DnsEndPoint("localhost", 6379));

        var connectionMultiplexer = ConnectionMultiplexer.Connect(redis);
        var database = connectionMultiplexer.GetDatabase();
        await GetPipeliningAsync(database);
    }

    public static async Task GetPipeliningAsync(IDatabase database)
    {
        var batch = database.CreateBatch();
        var redisValue = batch.HashGetAsync("test", "absexp");
        var redisValue2 = batch.HashGetAsync("test", "absexp");
        batch.Execute();
        var list = await redisValue;
        var list2 = await redisValue2;
    }

}
#pragma warning restore CS0618
