// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class RedisConfigurationOptionsTest : TestBase
{
    [TestMethod]
    public void Test()
    {
        DateTimeOffset absoluteExpiration = DateTimeOffset.UtcNow.AddDays(1);
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST),
                new(REDIS_HOST, 6379)
            },
            AbortOnConnectFail = true,
            AllowAdmin = true,
            ClientName = "test",
            ChannelPrefix = "ChannelPrefix",
            ConnectRetry = 5,
            ConnectTimeout = 3000,
            DefaultDatabase = 1,
            Password = "123456",
            Proxy = Proxy.Twemproxy,
            Ssl = true,
            SyncTimeout = 3000,
            AbsoluteExpiration = absoluteExpiration,
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromHours(3)
        };
        Assert.AreEqual(2, redisConfigurationOptions.Servers.Count);
        Assert.AreEqual(true, redisConfigurationOptions.AbortOnConnectFail);
        Assert.AreEqual(true, redisConfigurationOptions.AllowAdmin);
        Assert.AreEqual("test", redisConfigurationOptions.ClientName);
        Assert.AreEqual("ChannelPrefix", redisConfigurationOptions.ChannelPrefix);
        Assert.AreEqual(5, redisConfigurationOptions.ConnectRetry);
        Assert.AreEqual(3000, redisConfigurationOptions.ConnectTimeout);
        Assert.AreEqual(1, redisConfigurationOptions.DefaultDatabase);
        Assert.AreEqual("123456", redisConfigurationOptions.Password);
        Assert.AreEqual(Proxy.Twemproxy, redisConfigurationOptions.Proxy);
        Assert.AreEqual(true, redisConfigurationOptions.Ssl);
        Assert.AreEqual(3000, redisConfigurationOptions.SyncTimeout);
        Assert.AreEqual(absoluteExpiration, redisConfigurationOptions.AbsoluteExpiration);
        Assert.AreEqual(TimeSpan.FromHours(1), redisConfigurationOptions.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromHours(3), redisConfigurationOptions.SlidingExpiration);
    }

    [TestMethod]
    public void TestRedisServiceOptions()
    {
        var options = new RedisServerOptions(REDIS_HOST, 6379);

        Assert.AreEqual(REDIS_HOST, options.Host);
        Assert.AreEqual(6379, options.Port);

        Assert.ThrowsException<ArgumentException>(() => new RedisServerOptions("", 6379));
        Assert.ThrowsException<ArgumentException>(() => new RedisServerOptions(null!, 6379));
        Assert.ThrowsException<ArgumentException>(() => new RedisServerOptions(" ", 6379));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new RedisServerOptions(REDIS_HOST, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new RedisServerOptions(REDIS_HOST, -6379));


        options = new RedisServerOptions(REDIS_HOST);

        Assert.AreEqual(REDIS_HOST, options.Host);
        Assert.AreEqual(6379, options.Port);

        options = new RedisServerOptions("127.0.0.1:6378");

        Assert.AreEqual("127.0.0.1", options.Host);
        Assert.AreEqual(6378, options.Port);
    }

    [TestMethod]
    public void TestDistributedRedisCacheOptions()
    {
        RedisConfigurationOptions? redisConfigurationOptions = null;
        CacheEntryOptions? cacheEntryOptions = null;

        var distributedRedisCacheOptions = new DistributedRedisCacheOptions()
        {
            Options = redisConfigurationOptions,
            CacheEntryOptions = cacheEntryOptions
        };
        Assert.AreEqual(null, distributedRedisCacheOptions.Options);
        Assert.AreEqual(null, distributedRedisCacheOptions.CacheEntryOptions);

        redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            },
            Password = "123456"
        };
        cacheEntryOptions = new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        };
        distributedRedisCacheOptions = new DistributedRedisCacheOptions()
        {
            Options = redisConfigurationOptions,
            CacheEntryOptions = cacheEntryOptions
        };

        Assert.AreEqual(redisConfigurationOptions, distributedRedisCacheOptions.Options);
        Assert.AreEqual(cacheEntryOptions, distributedRedisCacheOptions.CacheEntryOptions);
    }
}
