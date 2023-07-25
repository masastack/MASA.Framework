// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class CustomLoginService : ICustomLoginService
{
    readonly ICaller _caller;

    public CustomLoginService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<CustomLoginModel?> GetCustomLoginByClientIdAsync(string environment, string clientId)
    {
        var requestUri = $"api/sso/customLogin/getByClientId";
        return await _caller.GetAsync<object, CustomLoginModel>(requestUri, new { environment, clientId });
    }
}
