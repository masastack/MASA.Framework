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
        var model = apiScope.ToModel();
        await _memoryCacheClient.SetAsync(key, model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<ApiScope> apiScopes)
    {
        var models = apiScopes.Select(apiScope => apiScope.ToModel());
        var data = models.ToDictionary(model => $"{CacheKeyConstants.API_SCOPE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(data);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(models, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        string key = $"{CacheKeyConstants.API_SCOPE_KEY}_{apiScope.Name}";
        await _memoryCacheClient.RemoveAsync<ApiScopeModel>(key);
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == apiScope.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<ApiScope> apiScopes)
    {
        var models = apiScopes.Select(apiScope => apiScope.ToModel());
        await UpdateListAsync(models);
        var map = models.ToDictionary(model => $"{CacheKeyConstants.API_SCOPE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(map);
    }

    private async Task UpdateListAsync(IEnumerable<ApiScopeModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_SCOPE_KEY, models);
    }
}
