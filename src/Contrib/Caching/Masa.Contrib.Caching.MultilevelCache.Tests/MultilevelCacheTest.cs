// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.Caching.MultilevelCache.Tests;

[TestClass]
public class MultilevelCacheTest : TestBase
{
    private static readonly FieldInfo SubscribeKeyTypeFieldInfo =
        typeof(MultilevelCacheClient).GetField("_subscribeKeyType", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly FieldInfo SubscribeKeyPrefixFieldInfo =
        typeof(MultilevelCacheClient).GetField("_subscribeKeyPrefix", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [TestMethod]
    public void TestAddMultilevelCache()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(_ =>
        {

        });

        Assert.IsTrue(services.Any<IMultilevelCacheClient>());
        Assert.IsTrue(services.Any<IMultilevelCacheClientFactory>());
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
        services.AddMultilevelCache("test", distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache());
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        using var multilevelCacheClient = multilevelCacheClientFactory.Create("test");
        Assert.IsNotNull(multilevelCacheClient);
    }

    [TestMethod]
    public void TestAddMultilevelCache4()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(distributedCacheBuilder => distributedCacheBuilder.UseStackExchangeRedisCache(),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.SpecificPrefix;
            });
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        using var multilevelCacheClient = multilevelCacheClientFactory.Create();
        Assert.IsNotNull(multilevelCacheClient);
    }

    [TestMethod]
    public void TestAddMultilevelCacheReturnIMultilevelCacheNotNull()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMultilevelCache(distributedCacheBuilder =>
        {
            distributedCacheBuilder.UseStackExchangeRedisCache(RedisConfigurationOptions);
        });
        builder.Services.AddMultilevelCache("test", distributedCacheBuilder =>
        {
            distributedCacheBuilder.UseStackExchangeRedisCache(RedisConfigurationOptions);
        }, globalOptions =>
        {
            globalOptions.CacheEntryOptions = new CacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            };
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
        var multilevelCacheClient = GetMultilevelCacheClient(serviceProvider.GetService<IManualMultilevelCacheClient>()!);
        using var multilevelCacheClient2 = multilevelCacheClientFactory.Create("test");
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
    public async Task TestModifyMultilevelCacheOptions()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMultilevelCache("test", distributedCacheBuilder
            => distributedCacheBuilder.UseStackExchangeRedisCache(RedisConfigurationOptions));
        var serviceProvider = builder.Services.BuildServiceProvider();

        var multilevelCacheClientFactory = serviceProvider.GetService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(multilevelCacheClientFactory);
        using var multilevelCacheClient = multilevelCacheClientFactory.Create();

        VerifyOriginal(multilevelCacheClient as MultilevelCacheClient);

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
        await Task.Delay(1000);
        VerifyOriginal(multilevelCacheClient as MultilevelCacheClient);

        VerifyTarget(serviceProvider.CreateScope().ServiceProvider.GetService<IMultilevelCacheClientFactory>()!.Create() as
                MultilevelCacheClient);

        await File.WriteAllTextAsync(Path.Combine(rootPath, "appsettings.json"), oldContent);

        void VerifyOriginal(MultilevelCacheClient? client)
        {
            Assert.IsNotNull(client);
            Assert.IsNotNull(client.GlobalCacheOptions);
            Assert.IsNotNull(client.GlobalCacheOptions.MemoryCacheEntryOptions);

            Assert.AreEqual(null, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
            Assert.AreEqual(TimeSpan.FromSeconds(30), client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
            Assert.AreEqual(TimeSpan.FromSeconds(50), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);

            string subscribeKeyPrefix = GetSubscribeKeyPrefix(client);
            Assert.AreEqual("masa", subscribeKeyPrefix);

            var subscribeKeyType = GetSubscribeKeyType(client);
            Assert.AreEqual(SubscribeKeyType.SpecificPrefix, subscribeKeyType);
        }

        void VerifyTarget(MultilevelCacheClient? client)
        {
            Assert.IsNotNull(client);

            var subscribeKeyPrefix = GetSubscribeKeyPrefix(client);
            Assert.AreEqual("masa1", subscribeKeyPrefix);

            var subscribeKeyType = GetSubscribeKeyType(client);
            Assert.AreEqual(SubscribeKeyType.ValueTypeFullNameAndKey, subscribeKeyType);
            Assert.IsNotNull(client.GlobalCacheOptions.MemoryCacheEntryOptions);
            Assert.AreEqual(dateNow, client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpiration);
            Assert.AreEqual(TimeSpan.FromHours(1), client.GlobalCacheOptions.MemoryCacheEntryOptions.AbsoluteExpirationRelativeToNow);
            Assert.AreEqual(TimeSpan.FromSeconds(60), client.GlobalCacheOptions.MemoryCacheEntryOptions.SlidingExpiration);
        }
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
                    {
                        "String", "s"
                    }
                };
            }
        );
        var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>();
        Assert.IsNotNull(factory);
        using var multilevelCacheClient = factory.Create();
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
                    {
                        "String", "s"
                    }
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

    private static string GetSubscribeKeyPrefix(MultilevelCacheClient client)
    {
        var subscribeKeyPrefix = (string?)SubscribeKeyPrefixFieldInfo.GetValue(client);
        Assert.IsNotNull(subscribeKeyPrefix);
        return subscribeKeyPrefix;
    }

    private static SubscribeKeyType GetSubscribeKeyType(MultilevelCacheClient client)
    {
        var subscribeKeyType = (SubscribeKeyType?)SubscribeKeyTypeFieldInfo.GetValue(client);
        Assert.IsNotNull(subscribeKeyType);
        return subscribeKeyType.Value;
    }

    private static IMultilevelCacheClient GetMultilevelCacheClient(IManualMultilevelCacheClient cacheClient)
    {
        return (IMultilevelCacheClient)typeof(DefaultMultilevelCacheClient).GetField("_cacheClient",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(cacheClient)!;
    }
}
#pragma warning restore CS0618
