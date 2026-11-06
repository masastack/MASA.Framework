// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class ApiScopeCacheTest
{
    IApiScopeCache _cache;
    ApiScope _apiScope;

    [TestInitialize]
    public void Initialized()
    {
        var options = new RedisConfigurationOptions()
        {
            // Each target framework uses its own Redis database, so test projects running concurrently on different TFMs do not share keys
            DefaultDatabase = Environment.Version.Major,
            ChannelPrefix = $"net{Environment.Version.Major}:",
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
        _cache = serviceCollection.BuildServiceProvider().GetRequiredService<IApiScopeCache>();
        _apiScope = new ApiScope("ApiScope");
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        await _cache.SetAsync(_apiScope);
        var apiScopes = await _cache.GetListAsync();
        Assert.IsTrue(apiScopes.Any(item => item.Name == _apiScope.Name));
    }

    [TestMethod]
    public async Task TestSetRangeAsync()
    {
        var input = new[] { _apiScope };
        await _cache.SetRangeAsync(input);
        var apiScopes = await _cache.GetListAsync();
        Assert.IsTrue(input.All(item => apiScopes.Any(item2 => item2.Name == item.Name)));
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        await _cache.SetAsync(_apiScope);
        var apiScopes = await _cache.GetListAsync(new[] { _apiScope.Name });
        Assert.IsTrue(apiScopes.All(item => item.Name == _apiScope.Name));
        apiScopes = await _cache.GetListAsync();
        Assert.IsTrue(apiScopes.Any(item => item.Name == _apiScope.Name));
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        await _cache.SetAsync(_apiScope);
        await _cache.RemoveAsync(_apiScope);
        var apiScopes = await _cache.GetListAsync();
        Assert.IsTrue(apiScopes.All(item => item.Name != _apiScope.Name));
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var input = new[] { _apiScope };
        await _cache.ResetAsync(input);
        var apiScopes = await _cache.GetListAsync();
        Assert.IsTrue(apiScopes.Select(item => item.Name).Except(input.Select(item => item.Name)).Any() is false);
    }
}
