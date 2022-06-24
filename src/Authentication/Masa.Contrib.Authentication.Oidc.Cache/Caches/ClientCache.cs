// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Caches;

public class ClientCache : IClientCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ClientCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<ClientModel?> GetAsync(string clientId)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{clientId}";
        return await _memoryCacheClient.GetAsync<ClientModel>(key);
    }

    public async Task SetAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.SetAsync(key, client.ToModel());
    }

    public async Task RemoveAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.RemoveAsync<ClientModel>(key);
    }

    public async Task SetRangeAsync(IEnumerable<Client> clients)
    {
        var data = clients.ToDictionary(client => $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}", client => client.ToModel());
        await _memoryCacheClient.SetListAsync(data);
    }

    public async Task ResetAsync(IEnumerable<Client> clients)
    {
        var models = clients.Select(client => client.ToModel());
        var data = models.ToDictionary(model => $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{model.ClientId}", model => model);
        await _memoryCacheClient.SetListAsync(data);
    }
}
