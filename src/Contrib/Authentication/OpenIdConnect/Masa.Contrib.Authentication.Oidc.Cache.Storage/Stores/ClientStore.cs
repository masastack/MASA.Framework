// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Storage.Stores;

public class ClientStore : IClientStore
{
    IClientCache _clientCache;

    public ClientStore(IClientCache clientCache)
    {
        _clientCache = clientCache;
    }

    public async Task<ClientModel?> FindClientByIdAsync(string clientId)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        var client = await _clientCache.GetAsync(clientId);
        return client;
    }
}
