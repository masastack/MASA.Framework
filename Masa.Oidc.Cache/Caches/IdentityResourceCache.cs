// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Oidc.Models.Models;

namespace Masa.Oidc.Cache.Caches;

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
        var identityResources = await _memoryCacheClient.GetListAsync<IdentityResourceModel>(keys.ToArray()) ?? new List<IdentityResourceModel>();
        return identityResources.Where(i => i is not null).ToList()!;
    }

    public async Task<List<IdentityResourceModel>> GetListAsync()
    {
        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResourceModel>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return identityResources;
    }

    public async Task AddOrUpdateAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        await _memoryCacheClient.SetAsync(key, new IdentityResourceModel(identityResource.Name, identityResource.DisplayName, identityResource.UserClaims.Select(uc => uc.UserClaim.Name).ToList())
        {
            Required = identityResource.Required,
            Emphasize = identityResource.Emphasize,
            Enabled = identityResource.Enabled,
            Description = identityResource.Description,
            ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
        });
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        string key = $"{CacheKeyConstants.IDENTITY_RESOURCE_KEY}_{identityResource.Name}";
        await _memoryCacheClient.RemoveAsync<IdentityResourceModel>(key);
    }
}
