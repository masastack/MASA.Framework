// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class DistributedCacheClientTest : TestBase
{
    private DistributedCacheClient _distributedCacheClient;
    private IDatabase _database;

    [TestInitialize]
    public void Initialize()
    {
        _distributedCacheClient = new DistributedCacheClient(GetConfigurationOptions());

        _database = ConnectionMultiplexer.Connect(GetConfigurationOptions()).GetDatabase();

        _distributedCacheClient.Set("test_caching", "1");
        _distributedCacheClient.Set("test_caching_2", Guid.Empty.ToString());
    }

    [DataTestMethod]
    [DataRow("cache_test", "content")]
    [DataRow("cache_test_2", "")]
    public async Task SetAsync(string key, string value)
    {
        await _distributedCacheClient.RemoveAsync(key);
        await _distributedCacheClient.SetAsync(key, value, new CacheEntryOptions<string>(TimeSpan.FromMinutes(1))
        {
            SlidingExpiration = TimeSpan.FromSeconds(30)
        });
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync(key));
        Assert.IsTrue(await _distributedCacheClient.GetAsync<string>(key) == value);
        await _distributedCacheClient.RemoveAsync(key);
        Assert.IsFalse(await _distributedCacheClient.ExistsAsync(key));
    }

    [DataTestMethod]
    [DataRow("cache_test_sync", "content")]
    public void Set(string key, string value)
    {
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value, new CacheEntryOptions<string>(TimeSpan.FromMinutes(1))
        {
            SlidingExpiration = TimeSpan.FromSeconds(30)
        });
        Assert.IsTrue(_distributedCacheClient.Exists(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<string>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<string?>(key));
        _distributedCacheClient.RemoveAsync(key);
        Assert.IsFalse(_distributedCacheClient.Exists(key));
    }

    [DataTestMethod]
    [DataRow("cache_test_sync", "content")]
    public void SetAndSpecifyTimeSpan(string key, string value)
    {
        _distributedCacheClient.Set(key, value, TimeSpan.FromSeconds(30));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 30 and >= 25);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("cache_test", "content")]
    public async Task SetAndSpecifyTimeSpanAsync(string key, string value)
    {
        await _distributedCacheClient.SetAsync(key, value, TimeSpan.FromSeconds(30));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 30 and >= 25);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync", "content")]
    public void SetAndSpecifyTime(string key, string value)
    {
        _distributedCacheClient.Set(key, value, DateTimeOffset.Now.AddMinutes(1));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 and >= 55);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("cache_test", "content")]
    public async Task SetAndSpecifyTimeAsync(string key, string value)
    {
        await _distributedCacheClient.SetAsync(key, value, DateTimeOffset.Now.AddMinutes(1));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 and >= 55);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync", "2022-01-01")]
    public void SetByDateTime(string key, string value)
    {
        var date = DateTime.Parse(value);
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, date);
        Assert.AreEqual(date, _distributedCacheClient.Get<DateTime>(key));
        Assert.AreEqual(date, _distributedCacheClient.Get<DateTime?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByGuid(string key)
    {
        var value = Guid.NewGuid();
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<Guid>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<Guid?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByDecimal(string key)
    {
        var value = 1.2M;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<decimal>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<decimal?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByDouble(string key)
    {
        var value = 1.2d;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<double>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<double?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByFloat(string key)
    {
        var value = 1.2f;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<float>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<float?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByInt(string key)
    {
        var value = 9;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<int>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<int?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByLong(string key)
    {
        var value = 9L;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<long>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<long?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByUShort(string key)
    {
        short value = 1;
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value);
        Assert.AreEqual(value, _distributedCacheClient.Get<short>(key));
        Assert.AreEqual(value, _distributedCacheClient.Get<short?>(key));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByStringArray(string key)
    {
        string[] values = new[] { "test1", "test2" };
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, values);

        var actualValues = _distributedCacheClient.Get<string[]>(key);
        Assert.IsNotNull(actualValues);
        Assert.AreEqual(values.Length, actualValues.Length);
        Assert.IsTrue(actualValues.Contains("test1") && actualValues.Contains("test2"));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_sync")]
    public void SetByStringCollection(string key)
    {
        List<string> values = new List<string>() { "test1", "test2" };
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, values);

        var actualValues = _distributedCacheClient.Get<string[]>(key);
        Assert.IsNotNull(actualValues);
        Assert.AreEqual(values.Count, actualValues.Length);
        Assert.IsTrue(actualValues.Contains("test1") && actualValues.Contains("test2"));
        _distributedCacheClient.RemoveAsync(key);
    }

    [TestMethod]
    public void SetList()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        _distributedCacheClient.SetList(dic!);

        Assert.IsTrue(_distributedCacheClient.Exists("setlist_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_3"));

        _distributedCacheClient.Remove("setlist_1");
        _distributedCacheClient.Remove("setlist_2");
        _distributedCacheClient.Remove("setlist_3");
    }

    [TestMethod]
    public async Task SetListAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        await _distributedCacheClient.SetListAsync(dic!);

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_3"));

        await _distributedCacheClient.RemoveAsync("setlist_1");
        await _distributedCacheClient.RemoveAsync("setlist_2");
        await _distributedCacheClient.RemoveAsync("setlist_3");
    }

    [TestMethod]
    public void SetListAndSpecifyTime()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        _distributedCacheClient.SetList(dic!, DateTimeOffset.Now.AddSeconds(30));

        Assert.IsTrue(_distributedCacheClient.Exists("setlist_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_3"));

        _distributedCacheClient.Remove("setlist_1");
        _distributedCacheClient.Remove("setlist_2");
        _distributedCacheClient.Remove("setlist_3");
    }

    [TestMethod]
    public async Task SetListAndSpecifyTimeAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        await _distributedCacheClient.SetListAsync(dic!, DateTimeOffset.Now.AddSeconds(30));

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_3"));

        await _distributedCacheClient.RemoveAsync("setlist_1");
        await _distributedCacheClient.RemoveAsync("setlist_2");
        await _distributedCacheClient.RemoveAsync("setlist_3");
    }

    [TestMethod]
    public void SetListAndSpecifyTimeSpan()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        _distributedCacheClient.SetList(dic!, TimeSpan.FromSeconds(30));

        Assert.IsTrue(_distributedCacheClient.Exists("setlist_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("setlist_3"));

        _distributedCacheClient.Remove("setlist_1");
        _distributedCacheClient.Remove("setlist_2");
        _distributedCacheClient.Remove("setlist_3");
    }

    [TestMethod]
    public async Task SetListAndSpecifyTimeSpanAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            { "setlist_1", Guid.NewGuid().ToString() },
            { "setlist_2", Guid.NewGuid().ToString() },
            { "setlist_3", Guid.NewGuid().ToString() }
        };
        await _distributedCacheClient.SetListAsync(dic!, TimeSpan.FromSeconds(30));

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("setlist_3"));

        await _distributedCacheClient.RemoveAsync("setlist_1");
        await _distributedCacheClient.RemoveAsync("setlist_2");
        await _distributedCacheClient.RemoveAsync("setlist_3");
    }

    [DataTestMethod]
    [DataRow("test_caching", "test_caching_2")]
    public void TestGetList(params string[] keys)
    {
        var list = _distributedCacheClient.GetList<string>(keys).ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("1", list[0]);
        Assert.AreEqual(Guid.Empty.ToString(), list[1]);
    }

    [DataTestMethod]
    [DataRow("test_caching", "test_caching_2")]
    public async Task TestGetListAsync(params string[] keys)
    {
        var list = (await _distributedCacheClient.GetListAsync<string>(keys)).ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("1", list[0]);
        Assert.AreEqual(Guid.Empty.ToString(), list[1]);
    }

    [DataTestMethod]
    [DataRow("test_1", "123")]
    [DataRow("test_2", "")]
    public void TestGetOrSet(string key, string value)
    {
        var res = _distributedCacheClient.GetOrSet(key, () =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return new CacheEntry<string>("", TimeSpan.FromSeconds(30));

            return new CacheEntry<string>(value, TimeSpan.FromHours(1));
        });

        Assert.AreEqual(value, res);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("test_1", "123")]
    [DataRow("test_2", "")]
    public async Task TestGetOrSetAsync(string key, string value)
    {
        var res = await _distributedCacheClient.GetOrSetAsync(key, () =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return new CacheEntry<string>("", TimeSpan.FromSeconds(30));

            return new CacheEntry<string>(value, TimeSpan.FromHours(1));
        });

        Assert.AreEqual(value, res);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test1", "test2")]
    [DataRow("test3")]
    public void TestRefresh(params string[] keys)
    {
        _distributedCacheClient.KeyExpire(keys, new CacheEntryOptions(TimeSpan.FromHours(1))
        {
            SlidingExpiration = TimeSpan.FromSeconds(600)
        });
        _distributedCacheClient.Refresh(keys);

        foreach (var key in keys)
        {
            var expireTimeSpan = _database.KeyTimeToLive(key);
            if (expireTimeSpan != null)
            {
                Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 600 and >= 595);
            }
        }
    }

    [DataTestMethod]
    [DataRow("test1", "test2")]
    [DataRow("test3")]
    public async Task TestRefreshAsync(params string[] keys)
    {
        await _distributedCacheClient.KeyExpireAsync(keys, new CacheEntryOptions(TimeSpan.FromHours(1))
        {
            SlidingExpiration = TimeSpan.FromSeconds(600)
        });
        await _distributedCacheClient.RefreshAsync(keys);

        foreach (var key in keys)
        {
            var expireTimeSpan = _database.KeyTimeToLive(key);
            if (expireTimeSpan != null)
            {
                Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 600 and >= 595);
            }
        }
    }

    [DataTestMethod]
    [DataRow("test_caching*", 2)]
    [DataRow("test_caching_*", 1)]
    [DataRow("ce*", 0)]
    public void TestGetKeys(string keyPattern, int count)
    {
        var list = _distributedCacheClient.GetKeys(keyPattern);
        Assert.AreEqual(count, _distributedCacheClient.GetKeys(keyPattern).Count());
    }

    [DataTestMethod]
    [DataRow("test_caching*", 2)]
    [DataRow("test_caching_*", 1)]
    [DataRow("ce*", 0)]
    public async Task TestGetKeysAsync(string keyPattern, int count)
    {
        Assert.AreEqual(count, (await _distributedCacheClient.GetKeysAsync(keyPattern)).Count());
    }

    [DataTestMethod]
    [DataRow("test_caching*", 2)]
    public void TestGetListByKeyPattern(string keyPattern, int count)
    {
        var list = _distributedCacheClient.GetByKeyPattern<string>(keyPattern);
        Assert.AreEqual(count, list.Count());

        Assert.IsTrue(list.Count(x
            => (x.Key == "test_caching" && x.Value == "1") || (x.Key == "test_caching_2" && x.Value == Guid.Empty.ToString())) == 2);
    }

    [DataTestMethod]
    [DataRow("test_caching*", 2)]
    public async Task TestGetListByKeyPatternAsync(string keyPattern, int count)
    {
        var list = await _distributedCacheClient.GetByKeyPatternAsync<string>(keyPattern);
        Assert.AreEqual(count, list.Count());

        Assert.IsTrue(list.Count(x
            => (x.Key == "test_caching" && x.Value == "1") || (x.Key == "test_caching_2" && x.Value == Guid.Empty.ToString())) == 2);
    }

    [DataTestMethod]
    [DataRow("hash_test", 1)]
    public async Task TestHashIncrementAsync(string key, int value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        Assert.AreEqual(value, await _distributedCacheClient.HashIncrementAsync(key, value));

        var result = await _distributedCacheClient.GetAsync<int>(key);
        Assert.IsTrue(result == value);

        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("hash_test", 0)]
    [DataRow("hash_test", -1)]
    public async Task TestHashIncrementAndValueLessThan1Async(string key, int value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async ()
            => await _distributedCacheClient.HashIncrementAsync(key, value));
    }

    [DataTestMethod]
    [DataRow("hash_test", 1)]
    [DataRow("hash_test", 0)]
    public async Task TestHashDecrementAsync(string key, long minVal)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await _distributedCacheClient.HashIncrementAsync(key, 2);
        Assert.AreEqual(1, await _distributedCacheClient.HashDecrementAsync(key, 1, minVal));
        var result = await _distributedCacheClient.GetAsync<int>(key);
        Assert.IsTrue(result == 1);

        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("hash_test", 0)]
    [DataRow("hash_test", -1)]
    [DataRow("hash_test", -2)]
    public async Task TestHashDecrementAndValueLessThan1Async(string key, long value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async ()
            => await _distributedCacheClient.HashDecrementAsync(key, value));
    }

    [DataTestMethod]
    [DataRow("hash_test", -1)]
    [DataRow("hash_test", -2)]
    public async Task TestHashDecrementAndMinalValueLessThan0Async(string key, long minVal)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async ()
            => await _distributedCacheClient.HashDecrementAsync(key, 1, minVal));
    }

    [DataTestMethod]
    [DataRow("test_expire")]
    public void TestKeyExpireAndSpecialTimeSpan(string key)
    {
        _distributedCacheClient.Set(key, "test_content");
        _distributedCacheClient.KeyExpire(key, TimeSpan.FromSeconds(30));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 30 and >= 25);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("test_expire")]
    public async Task TestKeyExpireAndSpecialTimeSpanAsync(string key)
    {
        await _distributedCacheClient.SetAsync(key, "test_content");
        await _distributedCacheClient.KeyExpireAsync(key, TimeSpan.FromSeconds(30));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 30 and >= 25);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test_expire")]
    public async Task TestKeyExpireAndSpecialTimeSpanAndKeyIsNotFoundAsync(string key)
    {
        await _distributedCacheClient.RemoveAsync(key);
        Assert.IsFalse(await _distributedCacheClient.KeyExpireAsync(key, TimeSpan.FromSeconds(30)));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNull(expireTimeSpan);
    }

    [DataTestMethod]
    [DataRow("test_expire")]
    public void TestKeyExpireAndSpecialTime(string key)
    {
        _distributedCacheClient.Set(key, "test_content");
        _distributedCacheClient.KeyExpire(key, DateTimeOffset.Now.AddMinutes(1));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 and >= 50);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("test_expire")]
    public async Task TestKeyExpireAndSpecialTimeAsync(string key)
    {
        await _distributedCacheClient.SetAsync(key, "test_content");
        await _distributedCacheClient.KeyExpireAsync(key, DateTimeOffset.Now.AddMinutes(1));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 and >= 50);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test_expire", "test_expire2")]
    public void TestMultiKeyExpireAndSpecialTime(string key, string key2)
    {
        _distributedCacheClient.Set(key, "test_content");
        _distributedCacheClient.Set(key2, "test_2_content");
        _distributedCacheClient.KeyExpire(new string[] { key, key2 }, new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(1)));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 and >= 55);
        expireTimeSpan = _database.KeyTimeToLive(key2);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is < 60 and > 55);
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Remove(key2);
    }

    [DataTestMethod]
    [DataRow("test_expire", "test_expire2")]
    public async Task TestMultiKeyExpireAndSpecialTimeAsync(string key, string key2)
    {
        await _distributedCacheClient.SetAsync(key, "test_content");
        await _distributedCacheClient.SetAsync(key2, "test_2_content");
        await _distributedCacheClient.KeyExpireAsync(new[] { key, key2 }, new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(5)));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 * 5 and >= 60 * 5 - 5);
        expireTimeSpan = await _database.KeyTimeToLiveAsync(key2);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 60 * 5 and >= 60 * 5 - 5);
        await _distributedCacheClient.RemoveAsync(key);
        await _distributedCacheClient.RemoveAsync(key2);
    }

    [DataTestMethod]
    [DataRow("test_expire", "test_expire2")]
    public async Task TestMultiKeyExpireAndKeyIsNotFoundAsync(string key, string key2)
    {
        await _distributedCacheClient.RemoveAsync(key);
        await _distributedCacheClient.RemoveAsync(key2);

        await _distributedCacheClient.SetAsync(key, "test_content");

        var res = await _distributedCacheClient.KeyExpireAsync(new[] { key, key2 },
            new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(5)));
        Assert.AreEqual(1, res);

        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test_chanel", "test_change_test", "2")]
    public void TestPublish(string channel, string key, string value)
    {
        int timer = 0;
        _distributedCacheClient.Subscribe<string>(channel, option =>
        {
            timer++;

            Assert.IsTrue(option.IsPublisherClient);
            Assert.IsTrue(option.Value == value);
        });

        _distributedCacheClient.Publish(channel, option =>
        {
            option.Operation = SubscribeOperation.Set;

            option.Key = key;
            option.Value = value;
        });

        Thread.Sleep(3000);
        Assert.IsTrue(timer == 1);
    }

    [DataTestMethod]
    [DataRow("test_chanel", "test_change_test", "2")]
    public async Task TestPublishAsync(string channel, string key, string value)
    {
        int timer = 0;
        await _distributedCacheClient.SubscribeAsync<string>(channel, option =>
        {
            timer++;

            Assert.IsTrue(option.IsPublisherClient);
            Assert.IsTrue(option.Value == value);
        });

        await _distributedCacheClient.PublishAsync(channel, option =>
        {
            option.Operation = SubscribeOperation.Set;

            option.Key = key;
            option.Value = value;
        });

        Thread.Sleep(3000);
        Assert.IsTrue(timer == 1);
    }

    [TestMethod]
    public void TestGetExpirationInSeconds()
    {
        DateTimeOffset creationTime = DateTimeOffset.Now;

        Assert.AreEqual(null, DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, null, null));

        Assert.AreEqual(1, DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, creationTime.AddSeconds(1), null));

        Assert.AreEqual(5, DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, null, TimeSpan.FromSeconds(5)));

        Assert.AreEqual(2,
            DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, creationTime.AddSeconds(2), TimeSpan.FromSeconds(5)));

        Assert.AreEqual(1,
            DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, creationTime.AddSeconds(3), TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public void TestGetAbsoluteExpiration()
    {
        var creationTime = DateTimeOffset.Now;
        var options = new CacheEntryOptions();
        Assert.AreEqual(null, options.GetAbsoluteExpiration(creationTime));

        options = new CacheEntryOptions(TimeSpan.FromSeconds(100));

        Assert.AreEqual(creationTime.Add(TimeSpan.FromSeconds(100)), options.GetAbsoluteExpiration(creationTime));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            options = new CacheEntryOptions(creationTime.AddSeconds(-1));
            options.GetAbsoluteExpiration(creationTime);
        });

        options = new CacheEntryOptions(creationTime.AddSeconds(1));
        Assert.AreEqual(creationTime.AddSeconds(1), options.GetAbsoluteExpiration(creationTime));
    }
}
