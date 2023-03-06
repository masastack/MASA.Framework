// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class ApiResourceCacheTest
{
    IApiResourceCache _cache;
    ApiResource _apiResource;

    [TestInitialize]
    public void Initialized()
    {
        var options = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>
            {
                new RedisServerOptions
                {
                    Host = "127.0.0.1",
                    Port = 6379
                }
            }
        };
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOidcCache(options);
        _cache = serviceCollection.BuildServiceProvider().GetRequiredService<IApiResourceCache>();
        _apiResource = new ApiResource("ApiResourceCache", "ApiResourceCache", "ApiResourceCache", "", default, default, default, default);
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        await _cache.SetAsync(_apiResource);
        var apiResources = await _cache.GetListAsync();
        Assert.IsTrue(apiResources.Any(item => item.Name == _apiResource.Name));
    }

    [TestMethod]
    public async Task TestSetRangeAsync()
    {
        var input = new[] { _apiResource };
        await _cache.SetRangeAsync(input);
        var apiResources = await _cache.GetListAsync();
        Assert.IsTrue(input.All(item => apiResources.Any(item2 => item2.Name == item.Name)));
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        await _cache.SetAsync(_apiResource);
        var apiResources = await _cache.GetListAsync(new[] { _apiResource.Name });
        Assert.IsTrue(apiResources.All(item => item.Name == _apiResource.Name));
        apiResources = await _cache.GetListAsync();
        Assert.IsTrue(apiResources.Any(item => item.Name == _apiResource.Name));
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        await _cache.SetAsync(_apiResource);
        await _cache.RemoveAsync(_apiResource);
        var apiResources = await _cache.GetListAsync();
        Assert.IsTrue(apiResources.All(item => item.Name != _apiResource.Name));
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var input = new[] { _apiResource };
        await _cache.ResetAsync(input);
        var apiResources = await _cache.GetListAsync();
        Assert.IsTrue(apiResources.Select(item => item.Name).Except(input.Select(item => item.Name)).Any() is false);
    }
}
