// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class DistributedCacheClientTest : TestBase
{
    private RedisCacheClient _distributedCacheClient;
    private IDatabase _database;

    [TestInitialize]
    public void Initialize()
    {
        _distributedCacheClient = new RedisCacheClient(GetConfigurationOptions());

        _database = ConnectionMultiplexer.Connect(GetConfigurationOptions()).GetDatabase();

        _distributedCacheClient.Set("test_caching", "1");
        _distributedCacheClient.Set("test_caching_2", Guid.Empty.ToString());
    }

    [DataTestMethod]
    [DataRow("cache_test_setasync", "content")]
    [DataRow("cache_test_setasync_1", "")]
    public async Task SetAsync(string key, string value)
    {
        await _distributedCacheClient.RemoveAsync(key);
        await _distributedCacheClient.SetAsync(key, value, new CacheEntryOptions(TimeSpan.FromMinutes(1))
        {
            SlidingExpiration = TimeSpan.FromSeconds(30)
        });
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync(key));
        Assert.IsTrue(await _distributedCacheClient.GetAsync<string>(key) == value);
        await _distributedCacheClient.RemoveAsync(key);
        Assert.IsFalse(await _distributedCacheClient.ExistsAsync(key));
    }

    [DataTestMethod]
    [DataRow("cache_test_set", "content")]
    public void Set(string key, string value)
    {
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, value, new CacheEntryOptions(TimeSpan.FromMinutes(1))
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
    [DataRow("cache_test_specify_time", "content")]
    public void SetAndSpecifyTimeSpan(string key, string value)
    {
        _distributedCacheClient.Set(key, value, TimeSpan.FromSeconds(30));
        var expireTimeSpan = _database.KeyTimeToLive(key);
        Assert.IsNotNull(expireTimeSpan);
        Assert.IsTrue(expireTimeSpan.Value.TotalSeconds is <= 30 and >= 25);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_specify_time_async", "content")]
    public async Task SetAndSpecifyTimeSpanAsync(string key, string value)
    {
        await _distributedCacheClient.SetAsync(key, value, TimeSpan.FromSeconds(30));
        CheckLifeCycle(_database, key, 25, 30);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_time", "content")]
    public void SetAndSpecifyTime(string key, string value)
    {
        _distributedCacheClient.Set(key, value, DateTimeOffset.Now.AddMinutes(1));
        CheckLifeCycle(_database, key, 55, 60);
        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_time_async", "content")]
    public async Task SetAndSpecifyTimeAsync(string key, string value)
    {
        await _distributedCacheClient.SetAsync(key, value, DateTimeOffset.Now.AddMinutes(1));
        CheckLifeCycle(_database, key, 55, 60);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_global_async", "content")]
    public async Task SetAndSpecifyTimeAsyncAndUseGlobalOptions(string key, string value)
    {
        var globalRedisConfigurationOptions = GetConfigurationOptions();
        globalRedisConfigurationOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
        var distributedCacheClient = new RedisCacheClient(globalRedisConfigurationOptions);
        var database = (await ConnectionMultiplexer.ConnectAsync(globalRedisConfigurationOptions)).GetDatabase();

        await distributedCacheClient.SetAsync(key, value);
        CheckLifeCycle(database, key, 55, 60);
        await distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_global", "content")]
    public void SetAndSpecifyTimeAndUseGlobalOptions(string key, string value)
    {
        var globalRedisConfigurationOptions = GetConfigurationOptions();
        globalRedisConfigurationOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
        var distributedCacheClient = new RedisCacheClient(globalRedisConfigurationOptions);
        var database = ConnectionMultiplexer.Connect(globalRedisConfigurationOptions).GetDatabase();

        distributedCacheClient.Set(key, value);
        CheckLifeCycle(database, key, 55, 60);
        distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_datetime", "2022-01-01")]
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
    [DataRow("cache_test_guid")]
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
    [DataRow("cache_test_decimal")]
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
    [DataRow("cache_test_double")]
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
    [DataRow("cache_test_float")]
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
    [DataRow("cache_test_int")]
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
    [DataRow("cache_test_long")]
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
    [DataRow("cache_test_ushort")]
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
    [DataRow("cache_test_string_array")]
    public void SetByStringArray(string key)
    {
        string[] values = new[]
        {
            "test1", "test2"
        };
        _distributedCacheClient.Remove(key);
        _distributedCacheClient.Set(key, values);

        var actualValues = _distributedCacheClient.Get<string[]>(key);
        Assert.IsNotNull(actualValues);
        Assert.AreEqual(values.Length, actualValues.Length);
        Assert.IsTrue(actualValues.Contains("test1") && actualValues.Contains("test2"));
        _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("cache_test_string_collection")]
    public void SetByStringCollection(string key)
    {
        List<string> values = new List<string>()
        {
            "test1",
            "test2"
        };
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
            {
                "SetList_1", Guid.NewGuid().ToString()
            },
            {
                "SetList_2", Guid.NewGuid().ToString()
            },
            {
                "SetList_3", Guid.NewGuid().ToString()
            }
        };
        _distributedCacheClient.SetList(dic!);

        Assert.IsTrue(_distributedCacheClient.Exists("SetList_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetList_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetList_3"));

        _distributedCacheClient.Remove("SetList_1");
        _distributedCacheClient.Remove("SetList_2");
        _distributedCacheClient.Remove("SetList_3");
    }

    [TestMethod]
    public async Task SetListAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            {
                "SetListAsync_1", Guid.NewGuid().ToString()
            },
            {
                "SetListAsync_2", Guid.NewGuid().ToString()
            },
            {
                "SetListAsync_3", Guid.NewGuid().ToString()
            }
        };
        await _distributedCacheClient.SetListAsync(dic!);

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAsync_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAsync_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAsync_3"));

        await _distributedCacheClient.RemoveAsync("SetListAsync_1");
        await _distributedCacheClient.RemoveAsync("SetListAsync_2");
        await _distributedCacheClient.RemoveAsync("SetListAsync_3");
    }

    [TestMethod]
    public void SetListAndSpecifyTime()
    {
        var dic = new Dictionary<string, string>()
        {
            {
                "SetListAndSpecifyTime_1", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTime_2", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTime_3", Guid.NewGuid().ToString()
            }
        };
        _distributedCacheClient.SetList(dic!, DateTimeOffset.Now.AddSeconds(30));

        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTime_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTime_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTime_3"));

        _distributedCacheClient.Remove("SetListAndSpecifyTime_1");
        _distributedCacheClient.Remove("SetListAndSpecifyTime_2");
        _distributedCacheClient.Remove("SetListAndSpecifyTime_3");
    }

    [TestMethod]
    public async Task SetListAndSpecifyTimeAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            {
                "SetListAndSpecifyTimeAsync_1", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeAsync_2", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeAsync_3", Guid.NewGuid().ToString()
            }
        };
        await _distributedCacheClient.SetListAsync(dic!, DateTimeOffset.Now.AddSeconds(30));

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeAsync_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeAsync_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeAsync_3"));

        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeAsync_1");
        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeAsync_2");
        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeAsync_3");
    }

    [TestMethod]
    public void SetListAndSpecifyTimeSpan()
    {
        var dic = new Dictionary<string, string>()
        {
            {
                "SetListAndSpecifyTimeSpan_1", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeSpan_2", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeSpan_3", Guid.NewGuid().ToString()
            }
        };
        _distributedCacheClient.SetList(dic!, TimeSpan.FromSeconds(30));

        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTimeSpan_1"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTimeSpan_2"));
        Assert.IsTrue(_distributedCacheClient.Exists("SetListAndSpecifyTimeSpan_3"));

        _distributedCacheClient.Remove("SetListAndSpecifyTimeSpan_1");
        _distributedCacheClient.Remove("SetListAndSpecifyTimeSpan_2");
        _distributedCacheClient.Remove("SetListAndSpecifyTimeSpan_3");
    }

    [TestMethod]
    public async Task SetListAndSpecifyTimeSpanAsync()
    {
        var dic = new Dictionary<string, string>()
        {
            {
                "SetListAndSpecifyTimeSpanAsync_1", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeSpanAsync_2", Guid.NewGuid().ToString()
            },
            {
                "SetListAndSpecifyTimeSpanAsync_3", Guid.NewGuid().ToString()
            }
        };
        await _distributedCacheClient.SetListAsync(dic!, TimeSpan.FromSeconds(30));

        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeSpanAsync_1"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeSpanAsync_2"));
        Assert.IsTrue(await _distributedCacheClient.ExistsAsync("SetListAndSpecifyTimeSpanAsync_3"));

        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeSpanAsync_1");
        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeSpanAsync_2");
        await _distributedCacheClient.RemoveAsync("SetListAndSpecifyTimeSpanAsync_3");
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
    [DataRow("test_caching_async", "test_caching_2_async")]
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
    [DataRow("test_3", null)]
    public void TestGetOrSet(string key, string? value)
    {
        var res = _distributedCacheClient.GetOrSet(key, () =>
        {
            if (value == null)
                return new CacheEntry<string?>(null);

            if (string.IsNullOrWhiteSpace(value))
                return new CacheEntry<string?>("", TimeSpan.FromSeconds(30));

            return new CacheEntry<string?>(value, TimeSpan.FromHours(1));
        });

        Assert.AreEqual(value, res);
        var expireTimeSpan = _database.KeyTimeToLive(key);


        _distributedCacheClient.Remove(key);
    }

    [DataTestMethod]
    [DataRow("test_1_async", "123_async")]
    [DataRow("test_2_async", "")]
    [DataRow("test_3_async", null)]
    public async Task TestGetOrSetAsync(string key, string? value)
    {
        var res = await _distributedCacheClient.GetOrSetAsync(key, () =>
        {
            if (value == null)
                return Task.FromResult(new CacheEntry<string?>(null));

            if (string.IsNullOrWhiteSpace(value))
                return Task.FromResult(new CacheEntry<string?>("", TimeSpan.FromSeconds(30)));

            return Task.FromResult(new CacheEntry<string?>(value, TimeSpan.FromHours(1)));
        });

        Assert.AreEqual(value, res);
        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test1_refresh", "test2_refresh")]
    [DataRow("test3_refresh")]
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
    [DataRow("test1_refresh_async", "test2_refresh_async")]
    [DataRow("test3_refresh_async")]
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
        Assert.AreEqual(count, list.Count());
    }

    [TestMethod]
    public void TestGetKeys2()
    {
        string key = "te" + Guid.NewGuid();
        var distributedCacheClient = new RedisCacheClient(new RedisConfigurationOptions()
        {
            GlobalCacheOptions = new CacheOptions()
            {
                CacheKeyType = CacheKeyType.TypeName
            }
        });
        distributedCacheClient.Set(key, new List<UserModel>()
        {
            new()
            {
                Name = "jim"
            }
        });

        var value = distributedCacheClient.Get<List<UserModel>>(key);
        Assert.IsNotNull(value);

        var keys = distributedCacheClient.GetKeys<List<UserModel>>("te*").ToList();
        Assert.AreEqual(1, keys.Count);

        distributedCacheClient.Remove<List<UserModel>>(key);
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
    [DataRow("hash_test_in", 1)]
    public async Task TestHashIncrementAsync(string key, int value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        Assert.AreEqual(value, await _distributedCacheClient.HashIncrementAsync(key, value));

        var result = await _distributedCacheClient.GetAsync<int>(key);
        Assert.IsTrue(result == value);

        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("hash_test_in1", 0)]
    [DataRow("hash_test_in1", -1)]
    public async Task TestHashIncrementAndValueLessThan1Async(string key, int value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await Assert.ThrowsExceptionAsync<MasaArgumentException>(async ()
            => await _distributedCacheClient.HashIncrementAsync(key, value));
    }

    [DataTestMethod]
    [DataRow("hash_test_d", 1)]
    [DataRow("hash_test_d", 0)]
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
    [DataRow("hash_test_d1", 0)]
    [DataRow("hash_test_d1", -1)]
    [DataRow("hash_test_d1", -2)]
    public async Task TestHashDecrementAndValueLessThan1Async(string key, long value)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await Assert.ThrowsExceptionAsync<MasaArgumentException>(async ()
            => await _distributedCacheClient.HashDecrementAsync(key, value));
    }

    [DataTestMethod]
    [DataRow("test_expire_special")]
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
    [DataRow("test_expire_special_async")]
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
    [DataRow("test_expire_notfound_async")]
    public async Task TestKeyExpireAndSpecialTimeSpanAndKeyIsNotFoundAsync(string key)
    {
        await _distributedCacheClient.RemoveAsync(key);
        Assert.IsFalse(await _distributedCacheClient.KeyExpireAsync(key, TimeSpan.FromSeconds(30)));
        var expireTimeSpan = await _database.KeyTimeToLiveAsync(key);
        Assert.IsNull(expireTimeSpan);
    }

    [DataTestMethod]
    [DataRow("test_expire_special_time_async")]
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
    [DataRow("test_expire_special_time")]
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
    [DataRow("m_test_expire_special_time", "m_test_expire2_special_time")]
    public void TestMultiKeyExpireAndSpecialTime(string key, string key2)
    {
        _distributedCacheClient.Set(key, "test_content");
        _distributedCacheClient.Set(key2, "test_2_content");
        _distributedCacheClient.KeyExpire(new string[]
        {
            key, key2
        }, new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(1)));
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
    [DataRow("m_test_expire_special_time_async", "m_test_expire2_special_time_async")]
    public async Task TestMultiKeyExpireAndSpecialTimeAsync(string key, string key2)
    {
        await _distributedCacheClient.SetAsync(key, "test_content");
        await _distributedCacheClient.SetAsync(key2, "test_2_content");
        await _distributedCacheClient.KeyExpireAsync(new[]
        {
            key, key2
        }, new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(5)));
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
    [DataRow("test_expire_notfound", "test_expire2_notfound")]
    public async Task TestMultiKeyExpireAndKeyIsNotFoundAsync(string key, string key2)
    {
        await _distributedCacheClient.RemoveAsync(key);
        await _distributedCacheClient.RemoveAsync(key2);

        await _distributedCacheClient.SetAsync(key, "test_content");

        var res = await _distributedCacheClient.KeyExpireAsync(new[]
            {
                key, key2
            },
            new CacheEntryOptions(DateTimeOffset.Now.AddMinutes(5)));
        Assert.AreEqual(1, res);

        await _distributedCacheClient.RemoveAsync(key);
    }

    [DataTestMethod]
    [DataRow("test_chanel", "test_change_test", "2")]
    public async Task TestPublishAsyncBySync(string channel, string key, string value)
    {
        int timer = 0;
        // ReSharper disable once MethodHasAsyncOverload
        _distributedCacheClient.Subscribe<string>(channel, option =>
        {
            timer++;

            Assert.IsTrue(option.IsPublisherClient);
            Assert.IsTrue(option.Value == value);
        });

        // ReSharper disable once MethodHasAsyncOverload
        _distributedCacheClient.Publish(channel, option =>
        {
            option.Operation = SubscribeOperation.Set;

            option.Key = key;
            option.Value = value;
        });

        await Task.Delay(1000);
        Assert.IsTrue(timer == 1);
    }

    [DataTestMethod]
    [DataRow("test_chanel_async", "test_change_test_async", "2")]
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

        await Task.Delay(1000);
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

    [TestMethod]
    public void TestExists()
    {
        var configurationOptions = GetConfigurationOptions();
        configurationOptions.GlobalCacheOptions = new CacheOptions()
        {
            CacheKeyType = CacheKeyType.TypeName
        };
        var distributedCacheClient = new RedisCacheClient(configurationOptions);
        var key = "redis.exist";
        distributedCacheClient.Set(key, "1");
        Assert.IsFalse(distributedCacheClient.Exists(key));

        Assert.IsTrue(distributedCacheClient.Exists<string>(key));
        Assert.IsTrue(distributedCacheClient.Exists<string>(key));

        distributedCacheClient.Remove(key);
        Assert.IsTrue(distributedCacheClient.Exists<string>(key));

        distributedCacheClient.Remove<string>(key);
        Assert.IsFalse(distributedCacheClient.Exists<string>(key));
    }

    [TestMethod]
    public async Task TestExistsAsync()
    {
        var configurationOptions = GetConfigurationOptions();
        configurationOptions.GlobalCacheOptions = new CacheOptions()
        {
            CacheKeyType = CacheKeyType.TypeName
        };
        var distributedCacheClient = new RedisCacheClient(configurationOptions);
        var key = "redis.exist_async";
        await distributedCacheClient.SetAsync(key, "1");
        Assert.IsFalse(await distributedCacheClient.ExistsAsync(key));

        Assert.IsTrue(await distributedCacheClient.ExistsAsync<string>(key));
        Assert.IsTrue(await distributedCacheClient.ExistsAsync<string>(key));

        await distributedCacheClient.RemoveAsync(key);
        Assert.IsTrue(await distributedCacheClient.ExistsAsync<string>(key));

        await distributedCacheClient.RemoveAsync<string>(key);
        Assert.IsFalse(await distributedCacheClient.ExistsAsync<string>(key));
    }
}
