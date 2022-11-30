// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests;

#pragma warning disable CS0618
[TestClass]
public class MultilevelCacheClientTest : TestBase
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
            new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            },
            "test");
        InitializeData();
    }

    [TestMethod]
    public void TestGet()
    {
        Assert.AreEqual("success", _multilevelCacheClient.Get<string>("test_multilevel_cache"));
        Assert.AreEqual(99.99m, _multilevelCacheClient.Get<decimal>("test_multilevel_cache_2"));

        _memoryCache.Remove(CacheKeyHelper.FormatCacheKey<decimal>("test_multilevel_cache_2", CacheKeyType.TypeName));
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

        CombinedCacheEntryOptions? combinedCacheEntryOptions = null;
        await _multilevelCacheClient.SetAsync(key, "test2", combinedCacheEntryOptions);
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

        key = "";
        Assert.ThrowsException<ArgumentException>(() => _multilevelCacheClient.Set(key, "success"));
    }

    [TestMethod]
    public void TestSetByCacheEntryOptionsIsNull()
    {
        string key = "test20";
        var multilevelCacheClient = InitializeByCacheEntryOptionsIsNull();
        Assert.AreEqual(null, multilevelCacheClient.Get<string>(key));
        CombinedCacheEntryOptions? combinedCacheEntryOptions = null;
        multilevelCacheClient.Set(key, "success", combinedCacheEntryOptions);
        Assert.AreEqual("success", multilevelCacheClient.Get<string>(key));
        multilevelCacheClient.Remove<string>(key);
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        string key = "test20";
        Assert.AreEqual(null, await _multilevelCacheClient.GetAsync<string>(key));
        await _multilevelCacheClient.SetAsync(key, "success", TimeSpan.FromSeconds(2));
        Assert.AreEqual("success", await _multilevelCacheClient.GetAsync<string>(key));
        await _multilevelCacheClient.RemoveAsync<string>(key);

        key = "";
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _multilevelCacheClient.SetAsync(key, "success"));
    }

    [TestMethod]
    public async Task TestSetByCacheEntryOptionsIsNullAsync()
    {
        string key = "test20";
        var multilevelCacheClient = InitializeByCacheEntryOptionsIsNull();
        Assert.AreEqual(null, await multilevelCacheClient.GetAsync<string>(key));
        await multilevelCacheClient.SetAsync(key, "success", TimeSpan.FromSeconds(2));
        Assert.AreEqual("success", await multilevelCacheClient.GetAsync<string>(key));
        await multilevelCacheClient.RemoveAsync<string>(key);
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
    public void TestSetListAndKeyValuesIsNull()
    {
        Dictionary<string, object> dictionary = null!;
        Assert.ThrowsException<ArgumentNullException>(() => _multilevelCacheClient.SetList(dictionary!, null));
    }

    [TestMethod]
    public void TestSetListByCacheEntryOptionsIsNull()
    {
        string key = "test20";
        string key2 = "test30";
        string key3 = "test40";
        var multilevelCacheClient = InitializeByCacheEntryOptionsIsNull();
        var list = multilevelCacheClient.GetList<string>(key, key2, key3).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(null, list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual(null, list[2]);

        multilevelCacheClient.SetList(new Dictionary<string, string?>()
        {
            { key, "success" },
            { key3, "success3" }
        });

        list = multilevelCacheClient.GetList<string>(key, key2, key3).ToList();

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual("success3", list[2]);
        multilevelCacheClient.Remove<string>(key);
        multilevelCacheClient.Remove<string>(key2);
        multilevelCacheClient.Remove<string>(key3);
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
        }, TimeSpan.FromMinutes(2));

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
    public async Task TestSetListAndKeyValuesIsNullAsync()
    {
        Dictionary<string, object> dictionary = null!;
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _multilevelCacheClient.SetListAsync(dictionary!, null));
    }

    [TestMethod]
    public async Task TestSetListByCacheEntryOptionsIsNullAsync()
    {
        string key = "test20";
        string key2 = "test30";
        string key3 = "test40";
        var multilevelCacheClient = InitializeByCacheEntryOptionsIsNull();
        var list = (await multilevelCacheClient.GetListAsync<string>(key, key2, key3)).ToList();
        Assert.AreEqual(3, list.Count);
        Assert.AreEqual(null, list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual(null, list[2]);

        await multilevelCacheClient.SetListAsync(new Dictionary<string, string?>()
        {
            { key, "success" },
            { key3, "success3" }
        });

        list = (await multilevelCacheClient.GetListAsync<string>(key, key2, key3)).ToList();

        Assert.AreEqual(3, list.Count);
        Assert.AreEqual("success", list[0]);
        Assert.AreEqual(null, list[1]);
        Assert.AreEqual("success3", list[2]);
        await multilevelCacheClient.RemoveAsync<string>(key);
        await multilevelCacheClient.RemoveAsync<string>(key2);
        await multilevelCacheClient.RemoveAsync<string>(key3);
    }

    [TestMethod]
    public void TestRefresh()
    {
        var keys = new List<string>()
        {
            "test20"
        };
        Mock<IMemoryCache> memoryCache = new();
        Mock<IDistributedCacheClient> distributedCacheClient = new();
        distributedCacheClient.Setup(client => client.Refresh<string>(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<CacheOptions>?>()))
            .Verifiable();
        memoryCache.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Verifiable();

        var multilevelCacheClient = new MultilevelCacheClient(memoryCache.Object,
            distributedCacheClient.Object,
            new CacheEntryOptions(TimeSpan.FromSeconds(10)),
            SubscribeKeyType.SpecificPrefix,
            new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            },
            "test");

        multilevelCacheClient.Refresh<string>(keys);

        memoryCache.Verify(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny), Times.Once);
        distributedCacheClient.Verify(client => client.Refresh<string>(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<CacheOptions>?>()),
            Times.Once);
    }

    [TestMethod]
    public async Task TestRefreshAsync()
    {
        var keys = new List<string>()
        {
            "test20"
        };
        Mock<IMemoryCache> memoryCache = new();
        Mock<IDistributedCacheClient> distributedCacheClient = new();
        distributedCacheClient.Setup(client
                => client.RefreshAsync<string>(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<CacheOptions>?>()))
            .Verifiable();
        memoryCache.Setup(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny)).Verifiable();

        var multilevelCacheClient = new MultilevelCacheClient(memoryCache.Object,
            distributedCacheClient.Object,
            new CacheEntryOptions(TimeSpan.FromSeconds(10)),
            SubscribeKeyType.SpecificPrefix,
            new CacheOptions()
            {
                CacheKeyType = CacheKeyType.None
            },
            "test");

        await multilevelCacheClient.RefreshAsync<string>(keys);

        memoryCache.Verify(cache => cache.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny), Times.Once);
        distributedCacheClient.Verify(
            client => client.RefreshAsync<string>(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<CacheOptions>?>()), Times.Once);
    }

    [TestMethod]
    public void TestGetMemoryCacheEntryOptions()
    {
        CacheEntryOptions? cacheEntryOptions = null;
        var customerDistributedCacheClient = new CustomerDistributedCacheClient(cacheEntryOptions);
        var options = customerDistributedCacheClient.GetBaseMemoryCacheEntryOptions(new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
            SlidingExpiration = TimeSpan.FromHours(1)
        });
        Assert.IsNotNull(options);
        Assert.AreEqual(TimeSpan.FromDays(1), options.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromHours(1), options.SlidingExpiration);
        Assert.AreEqual(null, options.AbsoluteExpiration);

        options = customerDistributedCacheClient.GetBaseMemoryCacheEntryOptions(null);
        Assert.IsNull(options);

        DateTimeOffset dateNow = DateTimeOffset.Now.AddDays(2);
        cacheEntryOptions = new CacheEntryOptions()
        {
            AbsoluteExpiration = dateNow,
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2),
            SlidingExpiration = TimeSpan.FromHours(3)
        };
        customerDistributedCacheClient = new CustomerDistributedCacheClient(cacheEntryOptions);
        options = customerDistributedCacheClient.GetBaseMemoryCacheEntryOptions(null);
        Assert.IsNotNull(options);
        Assert.AreEqual(TimeSpan.FromDays(2), options.AbsoluteExpirationRelativeToNow);
        Assert.AreEqual(TimeSpan.FromHours(3), options.SlidingExpiration);
        Assert.AreEqual(dateNow, options.AbsoluteExpiration);
    }

    private void InitializeData()
    {
        _distributedCacheClient.Set("test_multilevel_cache", "success");
        _distributedCacheClient.Set("test_multilevel_cache_2", 99.99m);
    }

    private static IMultilevelCacheClient InitializeByCacheEntryOptionsIsNull()
    {
        var services = new ServiceCollection();
        services.AddStackExchangeRedisCache("test", RedisConfigurationOptions).AddMultilevelCache(_ =>
        {
        });
        var serviceProvider = services.BuildServiceProvider();
        var cacheClientFactory = serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>();
        var multilevelCacheClient = cacheClientFactory.Create("test");
        return multilevelCacheClient;
    }

    [TestMethod]
    public void TestMultilevelCache()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(
            distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(_ =>
            {
            }),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.SubscribeKeyPrefix = "masa";
                multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.ValueTypeFullNameAndKey;
            });
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClient>();
        Assert.IsNotNull(multilevelCacheClient);
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        var services = new ServiceCollection();
        services.AddMultilevelCache(distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache());
        var serviceProvider = services.BuildServiceProvider();
        var multilevelCacheClient = serviceProvider.GetRequiredService<IMultilevelCacheClient>();
        string key = Guid.NewGuid().ToString();
        var value = await multilevelCacheClient.GetAsync<string?>(key);
        Assert.IsNull(value);

        await multilevelCacheClient.SetAsync<string>(key, "success");
        Assert.AreEqual("success", await multilevelCacheClient.GetAsync<string>(key));

        await multilevelCacheClient.RemoveAsync<string>(key);

        value = await multilevelCacheClient.GetAsync<string?>(key);
        Console.WriteLine("value: " + value);
        Assert.IsNull(value);
    }

    [TestMethod]
    public async Task TestSetBySpecifiedTimeAsync()
    {
        string key = Guid.NewGuid().ToString();
        var multilevelCacheClient = InitializeByCacheEntryOptionsIsNull();
        var value = await multilevelCacheClient.GetAsync<string>(key);
        Assert.AreEqual(null, value);
        await multilevelCacheClient.SetAsync(key, "success", TimeSpan.FromMilliseconds(500));

        value = await multilevelCacheClient.GetAsync<string>(key);
        Assert.AreEqual("success", value);

        await Task.Delay(1000);
        Assert.AreEqual(null, await multilevelCacheClient.GetAsync<string>(key));
    }
}
#pragma warning restore CS0618
