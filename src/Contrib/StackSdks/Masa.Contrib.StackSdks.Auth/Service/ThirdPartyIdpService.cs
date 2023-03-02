// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class ThirdPartyIdpService : IThirdPartyIdpService
{
    readonly ICaller _caller;
    readonly IMultilevelCacheClient _memoryCacheClient;

    public ThirdPartyIdpService(ICaller caller, IMultilevelCacheClient memoryCacheClient)
    {
        _caller = caller;
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ThirdPartyIdpModel>> GetAllAsync()
    {
        var requestUri = $"api/thirdPartyIdp/getAll";
        return await _caller.GetAsync<List<ThirdPartyIdpModel>>(requestUri) ?? new();
    }

    public async Task<List<ThirdPartyIdpModel>> GetAllFromCacheAsync()
    {
        var thirdPartyIdps = await _memoryCacheClient.GetAsync<List<ThirdPartyIdpModel>>(CacheKeyConsts.ALL_THIRD_PARTY_IDP);
        return thirdPartyIdps ?? new();
    }

    public async Task<LdapOptionsModel?> GetLdapOptionsAsync(string scheme)
    {
        var requestUri = $"api/thirdPartyIdp/ldapOptions";
        return await _caller.GetAsync<LdapOptionsModel>(requestUri, new { scheme });
    }
}
