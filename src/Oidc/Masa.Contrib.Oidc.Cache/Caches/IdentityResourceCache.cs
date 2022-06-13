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
        return identityResources.Where(i => i is not null).ToList()!;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync()
    {
        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResourceModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return identityResources;
    }

    public async Task SetAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        await _memoryCacheClient.SetAsync(key, identityResource.ToModel());
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        await _memoryCacheClient.RemoveAsync<IdentityResourceModel>(key);
    }

    public async Task AddAllAsync(IEnumerable<IdentityResource> identityResources)
    {
        await _memoryCacheClient.SetAsync(CacheKeyConstants.IDENTITY_RESOURCE_KEY, identityResources.Select(identityResource => identityResource.ToModel()));
    }

    public async Task SetRangeAsync(IEnumerable<IdentityResource> identityResources)
    {
        var data = identityResources.ToDictionary(idrs => $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{idrs.Name}", idrs => idrs.ToModel());
        await _memoryCacheClient.SetListAsync(data);
    }
}
