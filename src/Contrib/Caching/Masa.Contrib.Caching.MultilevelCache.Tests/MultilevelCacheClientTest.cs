// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests;

[TestClass]
public class MultilevelCacheClientTest
{
    private IMemoryCache _memoryCache;
    private IDistributedCacheClient _distributedCacheClient;
    private IMultilevelCacheClient _multilevelCacheClient;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        _memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        _distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClient>();
        _multilevelCacheClient = new MultilevelCacheClient(_memoryCache,
            _distributedCacheClient,
            new CacheEntryOptions(TimeSpan.FromSeconds(10)),
            SubscribeKeyType.SpecificPrefix,
            "test");
        InitializeData();
    }

    [TestMethod]
    public void TestGet()
    {
        Assert.AreEqual("success", _multilevelCacheClient.Get<string>("test"));
        Assert.AreEqual(99.99m, _multilevelCacheClient.Get<decimal>("test2"));
        Assert.AreEqual(null, _multilevelCacheClient.Get<string>("test10"));

        Assert.ThrowsException<ArgumentException>(() => _multilevelCacheClient.Get<string>(string.Empty));
    }


    private void InitializeData()
    {
        _distributedCacheClient.Set("test", "success");
        _distributedCacheClient.Set("test2", 99.99m);
    }
}
