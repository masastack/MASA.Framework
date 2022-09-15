// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Service;

public class ThirdPartyIdpCacheService : IThirdPartyIdpCacheService
{
    readonly ICallerFactory _callerFactory;
    readonly IMemoryCacheClient _memoryCacheClient;

    public ThirdPartyIdpCacheService(ICallerFactory callerFactory, IMemoryCacheClient memoryCacheClient)
    {
        _callerFactory = callerFactory;
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ThirdPartyIdpModel>> GetAllAsync()
    {
        var thirdPartyIdps = await _memoryCacheClient.GetAsync<List<ThirdPartyIdpModel>>(CacheKeyConsts.GETALLTHIRDPARTYIDP);
        if (thirdPartyIdps is null)
        {
            var caller = _callerFactory.Create(DEFAULT_CLIENT_NAME);
            var requestUri = $"api/thirdPartyIdp/getAll";
            thirdPartyIdps = await caller.GetAsync<List<ThirdPartyIdpModel>>(requestUri) ?? new();
            await _memoryCacheClient.SetAsync(CacheKeyConsts.GETALLTHIRDPARTYIDP, thirdPartyIdps);
        }
        return thirdPartyIdps;
    }
}
