// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

public class ApiResourceCache : IApiResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public ApiResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<ApiResourceModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => $"{CacheKeyConstants.API_RESOURCE_KEY}_{name}");
        var apiResources = await _memoryCacheClient.GetListAsync<ApiResourceModel>(keys.ToArray());
        return apiResources.Where(i => i is not null).ToList()!;
    }

    public async Task<List<ApiResourceModel>> GetListAsync()
    {
        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResourceModel>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        return apiResources;
    }

    public async Task SetAsync(ApiResource apiResource)
    {
        var model = apiResource.ToModel();
        string key = $"{CacheKeyConstants.API_RESOURCE_KEY}_{apiResource.Name}";
        await _memoryCacheClient.SetAsync(key, model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<ApiResource> apiResources)
    {
        var models = apiResources.Select(apiScope => apiScope.ToModel());
        var map = models.ToDictionary(model => $"{CacheKeyConstants.API_RESOURCE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(map);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(models, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        string key = $"{CacheKeyConstants.API_RESOURCE_KEY}_{apiResource.Name}";
        await _memoryCacheClient.RemoveAsync<ApiResourceModel>(key);
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == apiResource.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<ApiResource> apiResources)
    {
        var models = apiResources.Select(apiScope => apiScope.ToModel());
        await UpdateListAsync(models);
        var map = models.ToDictionary(model => $"{CacheKeyConstants.API_RESOURCE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(map);
    }

    private async Task UpdateListAsync(IEnumerable<ApiResourceModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.API_RESOURCE_KEY, models);
    }
}
