// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Caches;

public class ApiResourceCache : IApiResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiResourceModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => FormatKey(name));
        var apiResources = await _memoryCacheClient.GetListAsync<ApiResourceModel>(keys.ToArray());
        return apiResources.Where(apiResource => apiResource is not null).ToList()!;
    }

    public async Task<List<ApiResourceModel>> GetListAsync()
    {
        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResourceModel>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        return apiResources;
    }

    public async Task SetAsync(ApiResource apiResource)
    {
        var model = apiResource.ToModel();
        await _memoryCacheClient.SetAsync(FormatKey(apiResource), model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<ApiResource> apiResources)
    {
        var map = apiResources.ToDictionary(apiResource => FormatKey(apiResource), apiResource => apiResource.ToModel());
        await _memoryCacheClient.SetListAsync(map);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(map.Values, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        await _memoryCacheClient.RemoveAsync<ApiResourceModel>(FormatKey(apiResource));
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == apiResource.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<ApiResource> apiResources)
    {
        var map = apiResources.ToDictionary(apiResource => FormatKey(apiResource), apiResource => apiResource.ToModel());
        await _memoryCacheClient.SetListAsync(map);
        await UpdateListAsync(map.Values);
    }

    private async Task UpdateListAsync(IEnumerable<ApiResourceModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_RESOURCE_KEY, models);
    }

    private string FormatKey(ApiResource apiResources)
    {
        return FormatKey(apiResources.Name);
    }

    private string FormatKey(string name)
    {
        return $"{CacheKeyConstants.API_RESOURCE_KEY}_{name}";
    }
}
