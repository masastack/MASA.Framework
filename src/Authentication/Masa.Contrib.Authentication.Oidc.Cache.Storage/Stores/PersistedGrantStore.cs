// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Storage.Stores;

public class PersistedGrantStore : IPersistedGrantStore
{
    public Task<IEnumerable<PersistedGrantModel>> GetAllAsync(PersistedGrantFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<PersistedGrantModel> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAllAsync(PersistedGrantFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync(PersistedGrantModel grant)
    {
        throw new NotImplementedException();
    }
}
