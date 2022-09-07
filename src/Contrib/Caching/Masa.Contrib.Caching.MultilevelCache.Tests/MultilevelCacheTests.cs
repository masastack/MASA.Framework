// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests;

[TestClass]
public class MultilevelCacheTests : TestBase
{
    [TestMethod]
    public void TestAddMultilevelCache()
    {
        var services = new ServiceCollection();
        var cachingBuilder = Substitute.For<ICachingBuilder>();
        cachingBuilder.Name.Returns("test");
        cachingBuilder.Services.Returns(services);

        cachingBuilder.AddMultilevelCache();

        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClient>());
        Assert.IsTrue(cachingBuilder.Services.Any<IMultilevelCacheClientFactory>());
    }

    [TestMethod]
    public void TestAddMultilevelCacheBySpecialMultilevelCacheOptions()
    {
        var services = new ServiceCollection();
        var cachingBuilder = Substitute.For<ICachingBuilder>();
        cachingBuilder.Name.Returns("test");
        cachingBuilder.Services.Returns(services);

        cachingBuilder.AddMultilevelCache(new MultilevelCacheOptions());

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
        var multilevelCacheOptions = serviceProvider.GetService<IOptionsSnapshot<MultilevelCacheOptions>>();
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
        builder.Services.AddStackExchangeRedisCache(RedisConfigurationOptions).AddMultilevelCache();
        builder.Services.AddStackExchangeRedisCache("test", RedisConfigurationOptions).AddMultilevelCache(new MultilevelCacheOptions()
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
        Assert.IsNotNull(client.DefaultCacheEntryOptions);

        Assert.AreEqual(null, client.DefaultCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(TimeSpan.FromSeconds(30), client.DefaultCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(50), client.DefaultCacheEntryOptions.SlidingExpiration);

        var client2 = (MultilevelCacheClient)multilevelCacheClient2;
        Assert.IsNotNull(client2);
        Assert.IsNotNull(client2.DefaultCacheEntryOptions);

        Assert.AreEqual(null, client2.DefaultCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(null, client2.DefaultCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(10), client2.DefaultCacheEntryOptions.SlidingExpiration);
    }

    [TestMethod]
    public void TestAddMultilevelCacheRepeat()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddStackExchangeRedisCache("test", RedisConfigurationOptions)
            .AddMultilevelCache(new MultilevelCacheOptions()
            {
                CacheEntryOptions = new CacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                }
            }).AddMultilevelCache(new MultilevelCacheOptions()
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
        Assert.IsNotNull(client.DefaultCacheEntryOptions);

        Assert.AreEqual(null, client.DefaultCacheEntryOptions.AbsoluteExpiration);
        Assert.AreEqual(null, client.DefaultCacheEntryOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromSeconds(10), client.DefaultCacheEntryOptions.SlidingExpiration);
    }
}
