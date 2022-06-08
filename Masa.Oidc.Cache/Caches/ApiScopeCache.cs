// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Oidc.Models.Models;

namespace Masa.Oidc.Cache.Caches;

public class ApiScopeCache : IApiScopeCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiScopeCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiScopeModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => $"{CacheKeyConstants.API_SCOPE_KEY}_{name}");
        var apiScopes = await _memoryCacheClient.GetListAsync<ApiScopeModel>(keys.ToArray()) ?? new List<ApiScopeModel>();
        return apiScopes.Where(i => i is not null).ToList()!;
    }

    public async Task<List<ApiScopeModel>> GetListAsync()
    {
        var ApiScopes = await _memoryCacheClient.GetAsync<List<ApiScopeModel>>(CacheKeyConstants.API_SCOPE_KEY) ?? new();
        return ApiScopes;
    }

    public async Task AddOrUpdateAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Id}";
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
        string key = $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Id}";
        await _memoryCacheClient.RemoveAsync<ApiScopeModel>(key);
    }
}
