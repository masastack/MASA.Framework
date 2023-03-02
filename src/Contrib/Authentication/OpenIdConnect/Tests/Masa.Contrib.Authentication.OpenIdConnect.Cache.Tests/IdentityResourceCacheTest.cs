// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class IdentityResourceCacheTest
{
    IIdentityResourceCache _cache;
    IdentityResource _identityResource;

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
        _cache = serviceCollection.BuildServiceProvider().GetRequiredService<IIdentityResourceCache>();
        _identityResource = new IdentityResource("IdentityResource", "IdentityResource", "IdentityResource", default, default, default, default, default);
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        await _cache.SetAsync(_identityResource);
        var identityResources = await _cache.GetListAsync();
        Assert.IsTrue(identityResources.Any(item => item.Name == _identityResource.Name));
    }

    [TestMethod]
    public async Task TestSetRangeAsync()
    {
        var input = new[] { _identityResource };
        await _cache.SetRangeAsync(input);
        var identityResources = await _cache.GetListAsync();
        Assert.IsTrue(input.All(item => identityResources.Any(item2 => item2.Name == item.Name)));
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        await _cache.SetAsync(_identityResource);
        var identityResources = await _cache.GetListAsync(new[] { _identityResource.Name });
        Assert.IsTrue(identityResources.All(item => item.Name == _identityResource.Name));
        identityResources = await _cache.GetListAsync();
        Assert.IsTrue(identityResources.Any(item => item.Name == _identityResource.Name));
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        await _cache.SetAsync(_identityResource);
        await _cache.RemoveAsync(_identityResource);
        var identityResources = await _cache.GetListAsync();
        Assert.IsTrue(identityResources.All(item => item.Name != _identityResource.Name));
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var input = new[] { _identityResource };
        await _cache.ResetAsync(input);
        var identityResources = await _cache.GetListAsync();
        Assert.IsTrue(identityResources.Select(item => item.Name).Except(input.Select(item => item.Name)).Any() is false);
    }
}
