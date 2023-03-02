// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class ClientCacheTest
{
    IClientCache _cache;
    Client _client;

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
        _cache = serviceCollection.BuildServiceProvider().GetRequiredService<IClientCache>();
        _client = new Client(ClientTypes.Web, "Client", "Client");
    }

    [TestMethod]
    public async Task TestSetAsync()
    {
        await _cache.SetAsync(_client);
        var clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(clients.Any(item => item.ClientName == _client.ClientName));
    }

    [TestMethod]
    public async Task TestSetRangeAsync()
    {
        var input = new[] { _client };
        await _cache.SetRangeAsync(input);
        var clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(input.All(item => clients.Any(item2 => item2.ClientName == item.ClientName)));
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        await _cache.SetAsync(_client);
        var clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(clients.All(item => item.ClientName == _client.ClientName));
        clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(clients.Any(item => item.ClientName == _client.ClientName));
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        await _cache.SetAsync(_client);
        await _cache.RemoveAsync(_client);
        var clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(clients.All(item => item.ClientName != _client.ClientName));
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var input = new[] { _client };
        await _cache.ResetAsync(input);
        var clients = await _cache.GetListAsync(new[] { _client.ClientName });
        Assert.IsTrue(clients.Select(item => item.ClientName).Except(input.Select(item => item.ClientName)).Any() is false);
    }
}
