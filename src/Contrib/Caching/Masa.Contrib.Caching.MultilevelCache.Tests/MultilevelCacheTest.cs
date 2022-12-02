// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.Caching.MultilevelCache.Tests;

[TestClass]
public class MultilevelCacheTest : TestBase
{
    [TestMethod]
    public void TestAddMultilevelCache()
    {
        var services = new ServiceCollection();
        var cachingBuilder = Substitute.For<ICachingBuilder>();
        cachingBuilder.Name.Returns("test");
        cachingBuilder.Services.Returns(services);

        cachingBuilder.AddMultilevelCache(_ =>
        {
        });

        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClient>());
        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClientFactory>());
    }

    [TestMethod]
    public void TestAddMultilevelCache2()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache());
        var serviceProvider = services.BuildServiceProvider();

        Assert.IsNotNull(serviceProvider.GetService<IMultilevelCacheClient>());
        Assert.IsNotNull(serviceProvider.GetService<IMultilevelCacheClientFactory>());
    }

    [TestMethod]
    public void TestAddMultilevelCache3()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache("test", distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache());
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        Assert.IsNotNull(multilevelCacheClientFactory.Create("test"));
    }

    [TestMethod]
    public void TestAddMultilevelCache4()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(distributedCacheOptions =>
                distributedCacheOptions.UseStackExchangeRedisCache(),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.SpecificPrefix;
            });
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        Assert.IsNotNull(multilevelCacheClientFactory.Create());
    }

    [TestMethod]
    public void TestAddMultilevelCacheBySpecialMultilevelCacheOptions()
    {
        var services = new ServiceCollection();
        var cachingBuilder = Substitute.For<ICachingBuilder>();
        cachingBuilder.Name.Returns("test");
        cachingBuilder.Services.Returns(services);

        cachingBuilder.AddMultilevelCache(new MultilevelCacheGlobalOptions());

        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClient>());
        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClientFactory>());
    }

    [TestMethod]
    public void TestMultilevelCacheOptions()
    {
        var builder = WebApplication.CreateBuilder();
        var cachingBuilder = Substitute.For<ICachingBuilder>();
        cachingBuilder.Name.Returns("test");
        cachingBuilder.Services.Returns(builder.Services);
        cachingBuilder.AddMultilevelCache();

        var serviceProvider = builder.Services.BuildServiceProvider();
        var multilevelCacheOptions = serviceProvider.GetService<IOptionsSnapshot<MultilevelCacheGlobalOptions>>();
        Assert.IsNotNull(multilevelCacheOptions);
        Assert.IsNotNull(multilevelCacheOptions.Value);

        var option = multilevelCacheOptions.Get("test");
        Assert.IsNotNull(option);
        Assert.AreEqual("masa", option.SubscribeKeyPrefix);
        Assert.AreEqual(SubscribeKeyType.SpecificPrefix, option.SubscribeKeyType);
    }

    [TestMethod]
    public void TestAddMultilevelCacheReturnIMultilevelCacheNotNull()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddStackExchangeRedisCache(RedisConfigurationOptions).AddMultilevelCache().AddMultilevelCache();
        builder.Services.AddStackExchangeRedisCache("test", RedisConfigurationOptions).AddMultilevelCache(new MultilevelCacheGlobalOptions()
        {
            CacheEntryOptions = new CacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            }
        });

        var serviceProvider = builder.Services.BuildServiceProvider();
        var distributedCacheClientFactory = serviceProvider.GetService<IDistributedCacheClientFactory>();
        Assert.IsNotNull(distributedCacheClientFactory);
        var distributedCacheClient = serviceProvider.GetService<IDistributedCacheClient>();
        var distributedCacheClient2 = distributedCacheClientFactory.Create("test");

        Assert.IsNotNull(distributedCacheClient);
        Assert.IsNotNull(distributedCacheClient2);
        Assert.AreNotEqual(distributedCacheClient, distributedCacheClient2);

        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        var multilevelCacheClient = serviceProvider.GetService<IMultilevelCacheClient>();
        var multilevelCacheClient2 = multilevelCacheClientFactory.Create("test");
        Assert.IsNotNull(multilevelCacheClient);
        Assert.IsNotNull(multilevelCacheClient2);
        Assert.AreNotEqual(multilevelCacheClient, multilevelCacheClient2);

        var client = (MultilevelCacheClient)multilevelCacheClient;
        Assert.IsNotNull(client);
        Assert.IsNotNull(client.GlobalCacheOptions);
        Assert.IsNotNull(client.GlobalCacheOptions.MemoryCacheEntryOptions);

        Assert.AreEqual(null, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(TimeSpan.FromSeconds(30), client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(50), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);

        var client2 = (MultilevelCacheClient)multilevelCacheClient2;
        Assert.IsNotNull(client2);
        Assert.IsNotNull(client2.GlobalCacheOptions);
        Assert.IsNotNull(client2.GlobalCacheOptions.MemoryCacheEntryOptions);

        Assert.AreEqual(null, client2.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(null, client2.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(10), client2.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);
    }

    [TestMethod]
    public void TestAddMultilevelCacheRepeat()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddStackExchangeRedisCache("test", RedisConfigurationOptions)
            .AddMultilevelCache(multilevelCacheOptions =>
            {
                multilevelCacheOptions.CacheEntryOptions = new CacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                };
            }).AddMultilevelCache(new MultilevelCacheGlobalOptions()
            {
                CacheEntryOptions = new CacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                }
            });

        var serviceProvider = builder.Services.BuildServiceProvider();
        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        var multilevelCacheClient = multilevelCacheClientFactory.Create("test");

        var client = (MultilevelCacheClient)multilevelCacheClient;
        Assert.IsNotNull(client);
        Assert.IsNotNull(client.GlobalCacheOptions);
        Assert.IsNotNull(client.GlobalCacheOptions.MemoryCacheEntryOptions);

        Assert.AreEqual(null, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(null, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(10), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);
    }

    [TestMethod]
    public async Task TestModifyMultilevelCacheOptions()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddStackExchangeRedisCache("test", RedisConfigurationOptions)
            .AddMultilevelCache();
        var serviceProvider = builder.Services.BuildServiceProvider();

        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        var multilevelCacheClient = multilevelCacheClientFactory.Create();

        var client = (MultilevelCacheClient)multilevelCacheClient;
        Assert.IsNotNull(client);
        Assert.IsNotNull(client.GlobalCacheOptions);
        Assert.IsNotNull(client.GlobalCacheOptions.MemoryCacheEntryOptions);

        Assert.AreEqual(null, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(TimeSpan.FromSeconds(30), client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(50), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);

        var multilevelCacheClientType = typeof(MultilevelCacheClient);
        string subscribeKeyPrefix =
            (string)multilevelCacheClientType.GetField(
                    "_subscribeKeyPrefix",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(client)!;
        Assert.AreEqual("masa", subscribeKeyPrefix);

        SubscribeKeyType subscribeKeyType =
            (SubscribeKeyType)multilevelCacheClientType.GetField(
                    "_subscribeKeyType",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(client)!;
        Assert.AreEqual(SubscribeKeyType.SpecificPrefix, subscribeKeyType);

        var rootPath = builder.Environment.ContentRootPath;
        var oldContent = await File.ReadAllTextAsync(Path.Combine(rootPath, "appsettings.json"));

        var dateNow = DateTimeOffset.Now.AddDays(1);
        await File.WriteAllTextAsync(Path.Combine(rootPath, "appsettings.json"),
            System.Text.Json.JsonSerializer.Serialize(new
            {
                MultilevelCache = new MultilevelCacheGlobalOptions()
                {
                    SubscribeKeyPrefix = "masa1",
                    SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey,
                    CacheEntryOptions = new CacheEntryOptions()
                    {
                        AbsoluteExpiration = dateNow,
                        SlidingExpiration = TimeSpan.FromSeconds(60),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    }
                }
            }));
        Thread.Sleep(2000);

        subscribeKeyPrefix =
            (string)multilevelCacheClientType.GetField(
                    "_subscribeKeyPrefix",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(client)!;
        Assert.AreEqual("masa1", subscribeKeyPrefix);

        subscribeKeyType =
            (SubscribeKeyType)multilevelCacheClientType.GetField(
                    "_subscribeKeyType",
                    BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(client)!;
        Assert.AreEqual(SubscribeKeyType.ValueTypeFullNameAndKey, subscribeKeyType);
        Assert.AreEqual(dateNow, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(TimeSpan.FromHours(1), client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(60), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);

        await File.WriteAllTextAsync(Path.Combine(rootPath, "appsettings.json"), oldContent);
    }

    [TestMethod]
    public void TestFormatCacheKeyByTypeNameAlias()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(distributedCacheOptions =>
                distributedCacheOptions.UseStackExchangeRedisCache(RedisConfigurationOptions),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.GlobalCacheOptions = new CacheOptions()
                {
                    CacheKeyType = CacheKeyType.TypeAlias
                };
            },
            typeAliasOptions =>
            {
                typeAliasOptions.GetAllTypeAliasFunc = () => new Dictionary<string, string>()
                {
                    { "String", "s" }
                };
            }
        );
        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(factory);
        var multilevelCacheClient = factory.Create();
        var value = multilevelCacheClient.GetOrSet("configuration", () => new CacheEntry<string>("configuration json"));
        Assert.AreEqual("configuration json", value);
        multilevelCacheClient.Remove<string>("configuration");
    }


    [TestMethod]
    public void TestFormatCacheKeyByTypeNameAlias2()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMultilevelCache(distributedCacheOptions =>
                distributedCacheOptions.UseStackExchangeRedisCache(RedisConfigurationOptions),
            "MultilevelCache",
            typeAliasOptions =>
            {
                typeAliasOptions.GetAllTypeAliasFunc = () => new Dictionary<string, string>()
                {
                    { "String", "s" }
                };
            }
        );
        var serviceProvider = builder.Services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(factory);
        var multilevelCacheClient = factory.Create();
        var value = multilevelCacheClient.GetOrSet("configuration", () => new CacheEntry<string>("configuration json"));

        Assert.AreEqual("configuration json", value);
        multilevelCacheClient.Remove<string>("configuration");
    }
}
#pragma warning restore CS0618
