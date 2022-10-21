// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Caches;

[ExcludeFromCodeCoverage]
public class ApiScopeCache : IApiScopeCache
{
    private readonly IMultilevelCacheClient _memoryCacheClient;

    public ApiScopeCache(MemoryCacheProvider memoryCacheProvider)
    {
        _memoryCacheClient = memoryCacheProvider.GetMemoryCacheClient();
    }

    public async Task<List<ApiScopeModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(FormatKey).ToArray();
        var apiScopes = await _memoryCacheClient.GetListAsync<ApiScopeModel>(keys);
        return apiScopes.Where(apiScope => apiScope is not null).ToList()!;
    }

    public async Task<List<ApiScopeModel>> GetListAsync()
    {
        var apiScopes = await _memoryCacheClient.GetAsync<IEnumerable<ApiScopeModel>>(CacheKeyConstants.API_SCOPE_KEY);
        return apiScopes?.ToList() ?? new();
    }

    public async Task SetAsync(ApiScope apiScope)
    {
        var model = apiScope.ToModel();
        await _memoryCacheClient.SetAsync(FormatKey(apiScope), model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<ApiScope> apiScopes)
    {
        var map = apiScopes.ToDictionary(FormatKey, apiScope => apiScope.ToModel());
        await _memoryCacheClient.SetListAsync(map!);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(map.Values, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        await _memoryCacheClient.RemoveAsync<ApiScopeModel>(FormatKey(apiScope));
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == apiScope.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<ApiScope> apiScopes)
    {
        var map = apiScopes.ToDictionary(FormatKey, apiScope => apiScope.ToModel());
        await _memoryCacheClient.SetListAsync(map!);
        await UpdateListAsync(map.Values);
    }

    private async Task UpdateListAsync(IEnumerable<ApiScopeModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_SCOPE_KEY, models);
    }

    private string FormatKey(ApiScope apiScopes)
    {
        return FormatKey(apiScopes.Name);
    }

    private string FormatKey(string name)
    {
        return $"{CacheKeyConstants.API_SCOPE_KEY}_{name}";
    }
}
