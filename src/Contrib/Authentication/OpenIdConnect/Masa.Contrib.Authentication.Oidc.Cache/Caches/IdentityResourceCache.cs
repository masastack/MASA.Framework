// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Caches;

public class IdentityResourceCache : IIdentityResourceCache
{
    IMemoryCacheClient _memoryCacheClient;

    public IdentityResourceCache(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync(IEnumerable<string> names)
    {
        var keys = names.Select(name => FormatKey(name)).ToArray();
        var identityResources = await _memoryCacheClient.GetListAsync<IdentityResourceModel>(keys);
        return identityResources.Where(identityResource => identityResource is not null).ToList()!;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync()
    {
        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResourceModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return identityResources;
    }

    public async Task SetAsync(IdentityResource identityResource)
    {
        var model = identityResource.ToModel();
        await _memoryCacheClient.SetAsync(FormatKey(identityResource), model);
        // update list cache
        var list = await GetListAsync();
        list.Set(model, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task SetRangeAsync(IEnumerable<IdentityResource> identityResources)
    {
        var map = identityResources.ToDictionary(identityResource => FormatKey(identityResource), identityResource => identityResource.ToModel());
        await _memoryCacheClient.SetListAsync(map);
        // update list cache
        var list = await GetListAsync();
        list.SetRange(map.Values, item => item.Name);
        await UpdateListAsync(list);
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        await _memoryCacheClient.RemoveAsync<IdentityResourceModel>(FormatKey(identityResource));
        // update list cache
        var list = await GetListAsync();
        list.Remove(item => item.Name == identityResource.Name);
        await UpdateListAsync(list);
    }

    public async Task ResetAsync(IEnumerable<IdentityResource> identityResources)
    {
        var map = identityResources.ToDictionary(identityResource => FormatKey(identityResource), identityResource => identityResource.ToModel());
        await _memoryCacheClient.SetListAsync(map);        
        await UpdateListAsync(map.Values);
    }

    private async Task UpdateListAsync(IEnumerable<IdentityResourceModel> models)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.IDENTITY_RESOURCE_KEY, models);
    }

    private string FormatKey(IdentityResource identityResources)
    {
        return FormatKey(identityResources.Name);
    }

    private string FormatKey(string name)
    {
        return $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{name}";
    }
}
