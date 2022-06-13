// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

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

    public async Task AddOrUpdateAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.SetAsync(key, client.ToModel());
    }

    public async Task RemoveAsync(Client client)
    {
        string key = $"{CacheKeyConstants.CLIENT_KEY}_{client.ClientId}";
        await _memoryCacheClient.RemoveAsync<ClientModel>(key);
    }
}
