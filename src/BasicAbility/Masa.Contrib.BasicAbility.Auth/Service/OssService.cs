// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class OssService : IOssService
{
    readonly ICallerProvider _callerProvider;

    public OssService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    public async Task<SecurityTokenModel> GetSecurityTokenAsync()
    {
        var requestUri = $"api/oss/GetSecurityToken";
        return await _callerProvider.GetAsync<SecurityTokenModel>(requestUri) ?? throw new UserFriendlyException("If the fault is unknown, contact the administrator");
    }
}
