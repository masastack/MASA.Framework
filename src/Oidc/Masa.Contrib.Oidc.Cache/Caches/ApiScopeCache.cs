// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

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
        var apiScopes = await _memoryCacheClient.GetListAsync<ApiScopeModel>(keys.ToArray());
        return apiScopes.Where(i => i is not null).ToList()!;
    }

    public async Task<List<ApiScopeModel>> GetListAsync()
    {
        var ApiScopes = await _memoryCacheClient.GetAsync<List<ApiScopeModel>>(CacheKeyConstants.API_SCOPE_KEY) ?? new();
        return ApiScopes;
    }

    public async Task SetAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Name}";
        await _memoryCacheClient.SetAsync(key, apiScope.ToModel());
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Name}";
        await _memoryCacheClient.RemoveAsync<ApiScopeModel>(key);
    }

    public async Task AddAllAsync(IEnumerable<ApiScope> apiScopes)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_SCOPE_KEY, apiScopes.Select(apiScope => apiScope.ToModel()));
    }

    public async Task SetRangeAsync(IEnumerable<ApiScope> apiScopes)
    {
        var data = apiScopes.ToDictionary(apiScope => $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Name}", apiScope => apiScope.ToModel());
        await _memoryCacheClient.SetListAsync(data);
    }
}
