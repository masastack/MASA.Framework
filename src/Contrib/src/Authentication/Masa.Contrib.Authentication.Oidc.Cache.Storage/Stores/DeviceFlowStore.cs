// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Storage.Stores;

public class DeviceFlowStore : IDeviceFlowStore
{
    public Task<DeviceCodeModel> FindByDeviceCodeAsync(string deviceCode)
    {
        throw new NotImplementedException();
    }

    public Task<DeviceCodeModel> FindByUserCodeAsync(string userCode)
    {
        throw new NotImplementedException();
    }

    public Task RemoveByDeviceCodeAsync(string deviceCode)
    {
        throw new NotImplementedException();
    }

    public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCodeModel data)
    {
        throw new NotImplementedException();
    }

    public Task UpdateByUserCodeAsync(string userCode, DeviceCodeModel data)
    {
        throw new NotImplementedException();
    }
}
