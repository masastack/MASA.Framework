// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Oidc.Cache.Models;

namespace Masa.Oidc.Cache.Storage.Stores;

public class ClientStore : IClientStore
{
    IMemoryCacheClient _memoryCacheClient;

    public ClientStore(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<ClientModel?> FindClientByIdAsync(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var clients = await _memoryCacheClient.GetAsync<List<ClientModel>>(CacheKeyConstants.CLIENT_KEY) ?? new();
        return clients.FirstOrDefault(client => client.ClientId == clientId);
    }
}
