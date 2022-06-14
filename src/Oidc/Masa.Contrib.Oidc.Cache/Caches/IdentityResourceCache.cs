// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Caches;

public class IdentityResourceCache : IIdentityResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public IdentityResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{name}");
        var identityResources = await _memoryCacheClient.GetListAsync<IdentityResourceModel>(keys.ToArray());
        return identityResources.Where(identityResource => identityResource is not null).ToList()!;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync()
    {
        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResourceModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return identityResources;
    }

    public async Task SetAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        var model = identityResource.ToModel();
        await _memoryCacheClient.SetAsync(key, model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<IdentityResource> identityResources)
    {
        var models = identityResources.Select(identityResource => identityResource.ToModel());
        var map = models.ToDictionary(model => $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(map);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(models, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        await _memoryCacheClient.RemoveAsync<IdentityResourceModel>(key);
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == identityResource.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<IdentityResource> identityResources)
    {
        var models = identityResources.Select(identityResource => identityResource.ToModel());
        await UpdateListAsync(models);
        var data = models.ToDictionary(model => $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{model.Name}", model => model);
        await _memoryCacheClient.SetListAsync(data);
    }

    private async Task UpdateListAsync(IEnumerable<IdentityResourceModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.IDENTITY_RESOURCE_KEY, models);
    }
}
