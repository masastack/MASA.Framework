// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.Cache.Storage.Stores;

public class ClientStore : IClientStore
{
    IMemoryCacheClient _memoryCacheClient;

    public ClientStore(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<Client?> FindClientByIdAsync(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var clients = await _memoryCacheClient.GetAsync<List<Client>>(CacheKeyConstants.CLIENT_KEY) ?? new();
        return clients.FirstOrDefault(client => client.ClientId == clientId);
    }
}
