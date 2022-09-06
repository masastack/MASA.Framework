// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests;

[TestClass]
public class MultilevelCacheClientTest: TestBase
{
    private IMemoryCache _memoryCache;
    private IDistributedCacheClient _distributedCacheClient;
    private IMultilevelCacheClient _multilevelCacheClient;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache(RedisConfigurationOptions);
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
        Assert.AreEqual("success", _multilevelCacheClient.Get<string>("test_multilevel_cache"));
        Assert.AreEqual(99.99m, _multilevelCacheClient.Get<decimal>("test_multilevel_cache_2"));
        Assert.AreEqual(null, _multilevelCacheClient.Get<string>("test10"));

        Assert.ThrowsException<ArgumentException>(() => _multilevelCacheClient.Get<string>(string.Empty));
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        Assert.AreEqual("success", await _multilevelCacheClient.GetAsync<string>("test_multilevel_cache"));
        Assert.AreEqual(99.99m, await _multilevelCacheClient.GetAsync<decimal>("test_multilevel_cache_2"));
        Assert.AreEqual(null, await _multilevelCacheClient.GetAsync<string>("test10"));

        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _multilevelCacheClient.GetAsync<string>(string.Empty));
    }

    [TestMethod]
    public void TestGetAndSubscribe()
    {
        var key = "test200";
        string? value = string.Empty;
        value = _multilevelCacheClient.Get<string>(key, newVal =>
        {
            value = newVal;
        });
        Assert.AreEqual(null, value);

        _multilevelCacheClient.Set(key, "test2");
        Thread.Sleep(3000);
        Assert.AreEqual("test2", value);
        _multilevelCacheClient.Remove<string>(key);
    }

    [TestMethod]
    public async Task TestGetAndSubscribeAsync()
    {
        var key = "test200";
        string? value = await _multilevelCacheClient.GetAsync<string>(key, newVal =>
        {
            value = newVal;
        });
        Assert.AreEqual(null, value);

        await _multilevelCacheClient.SetAsync(key, "test2");
        Thread.Sleep(3000);
        Assert.AreEqual("test2", value);
        await _multilevelCacheClient.RemoveAsync<string>(key);
    }

    [TestMethod]
    public void TestGetList()
    {
        var list = _multilevelCacheClient.GetList<string>("test_multilevel_cache", "test10").ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var list = (await _multilevelCacheClient.GetListAsync<string>("test_multilevel_cache", "test10")).ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
    }

    [TestMethod]
    public void TestGetOrSet()
    {
        Assert.AreEqual("success", _multilevelCacheClient.GetOrSet("test100", new CombinedCacheEntry<string>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<string>("success")
        }));
        Assert.AreEqual(99.9m, _multilevelCacheClient.GetOrSet("test101", new CombinedCacheEntry<decimal?>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<decimal?>(99.9m)
        }));
        Assert.AreEqual(99.9m, _multilevelCacheClient.GetOrSet("test102", new CombinedCacheEntry<decimal>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<decimal>(99.9m)
        }));

        var guid = Guid.NewGuid();
        Assert.AreEqual(guid, _multilevelCacheClient.GetOrSet("test103", new CombinedCacheEntry<Guid?>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<Guid?>(guid, TimeSpan.FromSeconds(3))
        }));
        Assert.AreEqual(guid, _multilevelCacheClient.GetOrSet("test104", new CombinedCacheEntry<Guid>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<Guid>(guid, TimeSpan.FromSeconds(3))
        }));
        _multilevelCacheClient.Remove<string>("test100");
        _multilevelCacheClient.Remove<decimal?>("test101");
        _multilevelCacheClient.Remove<decimal>("test102");
        _multilevelCacheClient.Remove<Guid?>("test103");
        _multilevelCacheClient.Remove<Guid?>("test104");
    }

    [TestMethod]
    public async Task TestGetOrSetAsync()
    {
        Assert.AreEqual("success", await _multilevelCacheClient.GetOrSetAsync("test100", new CombinedCacheEntry<string>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<string>("success")
        }));
        Assert.AreEqual(99.9m, await _multilevelCacheClient.GetOrSetAsync("test101", new CombinedCacheEntry<decimal?>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<decimal?>(99.9m)
        }));

        var guid = Guid.NewGuid();
        Assert.AreEqual(guid, await _multilevelCacheClient.GetOrSetAsync("test102", new CombinedCacheEntry<Guid?>()
        {
            DistributedCacheEntryFunc = () => new CacheEntry<Guid?>(guid, TimeSpan.FromSeconds(3))
        }));

        await _multilevelCacheClient.RemoveAsync<string>("test100");
        await _multilevelCacheClient.RemoveAsync<decimal?>("test101");
        await _multilevelCacheClient.RemoveAsync<Guid?>("test102");
    }

    [TestMethod]
    public void TestSet()
    {
        string key = "test20";
        Assert.AreEqual(null, _multilevelCacheClient.Get<string>(key));
        _multilevelCacheClient.Set(key, "success", TimeSpan.FromSeconds(5));
        Assert.AreEqual("success", _multilevelCacheClient.Get<string>(key));
        _multilevelCacheClient.Remove<string>(key);
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        string key = "test20";
        Assert.AreEqual(null, await _multilevelCacheClient.GetAsync<string>(key));
        await _multilevelCacheClient.SetAsync(key, "success", TimeSpan.FromSeconds(2));
        Assert.AreEqual("success", await _multilevelCacheClient.GetAsync<string>(key));
        await _multilevelCacheClient.RemoveAsync<string>(key);
    }

    [TestMethod]
    public void TestSetList()
    {
        string key = "test20";
        string key2 = "test30";
        string key3 = "test40";
        var list = _multilevelCacheClient.GetList<string>(key, key2, key3).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(null, list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual(null, list[2]);

        _multilevelCacheClient.SetList(new Dictionary<string, string?>()
        {
            { key, "success" },
            { key3, "success3" }
        }, TimeSpan.FromSeconds(2));

        list = _multilevelCacheClient.GetList<string>(key, key2, key3).ToList();

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual("success3", list[2]);
        _multilevelCacheClient.Remove<string>(key);
        _multilevelCacheClient.Remove<string>(key2);
        _multilevelCacheClient.Remove<string>(key3);
    }

    [TestMethod]
    public async Task TestSetListAsync()
    {
        string key = "test20";
        string key2 = "test30";
        string key3 = "test40";
        var list = (await _multilevelCacheClient.GetListAsync<string>(key, key2, key3)).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(null, list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual(null, list[2]);

        await _multilevelCacheClient.SetListAsync(new Dictionary<string, string?>()
        {
            { key, "success" },
            { key3, "success3" }
        }, TimeSpan.FromSeconds(2));

        list = (await _multilevelCacheClient.GetListAsync<string>(key, key2, key3)).ToList();

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual("success3", list[2]);
        await _multilevelCacheClient.RemoveAsync<string>(key);
        await _multilevelCacheClient.RemoveAsync<string>(key2);
        await _multilevelCacheClient.RemoveAsync<string>(key3);
    }

    [TestMethod]
    public void TestRefresh()
    {
        var keys = new List<string>()
        {
            "test20"
        };
        var distributedCacheClient = Substitute.For<IDistributedCacheClient>();

        var memoryCache = Substitute.For<IMemoryCache>();
        var multilevelCacheClient = new MultilevelCacheClient(memoryCache,
            distributedCacheClient,
            new CacheEntryOptions(TimeSpan.FromSeconds(10)),
            SubscribeKeyType.SpecificPrefix,
            "test");

        multilevelCacheClient.Refresh<string>(keys);

        Received.InOrder(() =>
        {
            distributedCacheClient.Refresh(keys);

            Parallel.ForEach(keys, key =>
            {
                _memoryCache.TryGetValue(SubscribeHelper.FormatMemoryCacheKey<string>(key), out _);
            });
        });
    }

    [TestMethod]
    public async Task TestRefreshAsync()
    {
        var keys = new List<string>()
        {
            "test20"
        };
        var distributedCacheClient = Substitute.For<IDistributedCacheClient>();

        var memoryCache = Substitute.For<IMemoryCache>();
        var multilevelCacheClient = new MultilevelCacheClient(memoryCache,
            distributedCacheClient,
            new CacheEntryOptions(TimeSpan.FromSeconds(10)),
            SubscribeKeyType.SpecificPrefix,
            "test");

        await multilevelCacheClient.RefreshAsync<string>(keys);

        Received.InOrder(async () =>
        {
            await distributedCacheClient.RefreshAsync(keys);

            Parallel.ForEach(keys, key =>
            {
                _memoryCache.TryGetValue(SubscribeHelper.FormatMemoryCacheKey<string>(key), out _);
            });
        });
    }

    private void InitializeData()
    {
        _distributedCacheClient.Set("test_multilevel_cache", "success");
        _distributedCacheClient.Set("test_multilevel_cache_2", 99.99m);
    }
}
