// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Caches;

public class ClientCache : IClientCache
{
    private readonly IMultilevelCacheClient _memoryCacheClient;

    public ClientCache(MemoryCacheProvider memoryCacheProvider)
    {
        _memoryCacheClient = memoryCacheProvider.GetMemoryCacheClient();
    }

    public async Task<ClientModel?> GetAsync(string clientId)
    {
        return await _memoryCacheClient.GetAsync<ClientModel>(FormatKey(clientId));
    }

    public async Task<List<ClientModel>> GetListAsync(IEnumerable<string> clientIds)
    {
        var keys = clientIds.Select(clientId => FormatKey(clientId)).ToArray();
        var clients = await _memoryCacheClient.GetListAsync<ClientModel>(keys);
        return clients.Where(client => client is not null).ToList()!;
    }

    public async Task SetAsync(Client client)
    {
        await _memoryCacheClient.SetAsync(FormatKey(client), client.ToModel());
    }

    public async Task RemoveAsync(Client client)
    {
        await _memoryCacheClient.RemoveAsync<ClientModel>(FormatKey(client));
    }

    public async Task SetRangeAsync(IEnumerable<Client> clients)
    {
        var data = clients.ToDictionary(client => FormatKey(client), client => client.ToModel());
        await _memoryCacheClient.SetListAsync(data);
    }

    public async Task ResetAsync(IEnumerable<Client> clients)
    {
        var data = clients.ToDictionary(client => FormatKey(client), client => client.ToModel());
        await _memoryCacheClient.SetListAsync(data);
    }

    private string FormatKey(Client client)
    {
        return FormatKey(client.ClientId);
    }

    private string FormatKey(string clientId)
    {
        return $"{CacheKeyConstants.CLIENT_KEY}_{clientId}";
    }
}
