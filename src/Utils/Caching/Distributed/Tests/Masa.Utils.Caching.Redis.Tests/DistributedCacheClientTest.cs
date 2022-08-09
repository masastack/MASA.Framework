// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis.Tests;

[TestClass]
public class DistributedCacheClientTest
{
    private IServiceProvider _serviceProvider;
    private IDistributedCacheClient _cacheClient;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddMasaRedisCache(option =>
        {
            option.Servers = new List<RedisServerOptions>()
            {
                new("localhost", 6379),
            };
        });
        _serviceProvider = services.BuildServiceProvider();
        _cacheClient = _serviceProvider.GetRequiredService<IDistributedCacheClient>();
        _cacheClient.Remove<string>("test1", "test2", "redis1", "redis2");
    }

    [TestMethod]
    public async Task TestGetKeysAsyncReturnCountIs2()
    {
        Assert.IsNotNull(_cacheClient);
        _cacheClient.Set("test1", "test1");
        _cacheClient.Set("test2", "test2");
        _cacheClient.Set("redis1", "redis1");
        _cacheClient.Set("redis2", "redis2");
        var keys = await _cacheClient.GetKeysAsync("test*");
        Assert.AreEqual(2, keys.Count);
    }

    [TestMethod]
    public void TestGetListByKeyPatternReturnCountIs1()
    {
        _cacheClient.Set("test1", "test1:Result");
        _cacheClient.Set("redis1", "redis1");
        _cacheClient.Set("redis2", "redis2");
        var dictionary = _cacheClient.GetListByKeyPattern<string>("test*");
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary["test1"] == "test1:Result");
    }

    [TestMethod]
    public async Task TestGetListByKeyPatternAsyncReturnCountIs1()
    {
        _cacheClient.Set("test1", "test1:Result");
        _cacheClient.Set("redis1", "redis1");
        _cacheClient.Set("redis2", "redis2");
        var dictionary = await _cacheClient.GetListByKeyPatternAsync<string>("test*");
        Assert.AreEqual(1, dictionary.Count);
        Assert.IsTrue(dictionary["test1"] == "test1:Result");
    }
}
