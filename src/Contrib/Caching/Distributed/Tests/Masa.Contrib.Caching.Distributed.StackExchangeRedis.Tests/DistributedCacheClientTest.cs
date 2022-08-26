// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Tests;

[TestClass]
public class DistributedCacheClientTest : TestBase
{
    private DistributedCacheClient _distributedCacheClient;

    [TestInitialize]
    public void Initialize()
    {
        //todo: waiting for initialization
        // _distributedCacheClient = new DistributedCacheClient(GetConfigurationOptions(), GetJsonSerializerOptions());

        _distributedCacheClient.Set("test", 1);
        _distributedCacheClient.Set("test2", 2);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        string key = "test";
        int value = 1;
        await _distributedCacheClient.SetAsync(key, value, new CacheEntryOptions<int>()
        {
            SlidingExpiration = TimeSpan.FromDays(1)
        });

        var list = (await _distributedCacheClient.GetListAsync<int>(key)).ToList();
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(1, list[0]);
        //todo: Waiting to delete data
    }

    [DataTestMethod]
    [DataRow("test", "test2")]
    public async Task TestRefreshAsync(params string[] keys)
    {
        await _distributedCacheClient.RefreshAsync(keys);
    }
}
