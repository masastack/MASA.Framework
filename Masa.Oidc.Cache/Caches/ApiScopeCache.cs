// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.Cache.Caches;

public class ApiScopeCache : IApiScopeCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiScopeCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiScopeModel>> GetListAsync()
    {
        var ApiScopes = await _memoryCacheClient.GetAsync<List<ApiScopeModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return ApiScopes;
    }

    public async Task AddOrUpdateAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{apiScope.Id}";
        await _memoryCacheClient.SetAsync(key, new ApiScopeModel(apiScope.Name, apiScope.DisplayName, apiScope.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Required = apiScope.Required,
            Emphasize = apiScope.Emphasize,
            Enabled = apiScope.Enabled,
            Description = apiScope.Description,
            ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
        });
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{apiScope.Id}";
        await _memoryCacheClient.RemoveAsync<ApiScopeModel>(key);
    }
}
