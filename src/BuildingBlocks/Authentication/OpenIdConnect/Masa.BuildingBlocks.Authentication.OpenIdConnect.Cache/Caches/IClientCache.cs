// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Cache.Caches;

public interface IClientCache
{
    Task<ClientModel?> GetAsync(string clientId);

    Task<List<ClientModel>> GetListAsync(IEnumerable<string> clientIds);

    Task SetAsync(Client client);

    Task SetRangeAsync(IEnumerable<Client> clients);

    Task RemoveAsync(Client client);

    Task ResetAsync(IEnumerable<Client> clients);
}
