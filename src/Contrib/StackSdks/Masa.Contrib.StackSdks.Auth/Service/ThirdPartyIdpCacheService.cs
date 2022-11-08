// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class ThirdPartyIdpCacheService : IThirdPartyIdpCacheService
{
    readonly IMultilevelCacheClient _memoryCacheClient;

    public ThirdPartyIdpCacheService(AuthClientMultilevelCacheProvider authClientMultilevelCacheProvider)
    {
        _memoryCacheClient = authClientMultilevelCacheProvider.GetMultilevelCacheClient();
    }

    public async Task<List<ThirdPartyIdpModel>> GetAllAsync()
    {
        var thirdPartyIdps = await _memoryCacheClient.GetAsync<List<ThirdPartyIdpModel>>(CacheKeyConsts.ALL_THIRD_PARTYIDP);
        return thirdPartyIdps ?? new();
    }
}
