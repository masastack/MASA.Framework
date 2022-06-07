// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Oidc.Storage.Models;

namespace Masa.Oidc.Cache.Storage.Stores;

public class ResourceStore : IResourceStore
{
    IMemoryCacheClient _memoryCacheClient;

    public ResourceStore(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        ArgumentNullException.ThrowIfNull(apiResourceNames);

        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResource>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        return apiResources.Where(apiResource => apiResourceNames.Contains(apiResource.Name));
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResource>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        return apiResources.Where(apiResource => apiResource.Scopes?.Any(scope => scopeNames.Contains(scope)) is true);
    }

    public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var apiScopes = await _memoryCacheClient.GetAsync<List<ApiScope>>(CacheKeyConstants.API_SCOPE_KEY) ?? new();
        return apiScopes.Where(apiScope => scopeNames.Contains(apiScope.Name));
    }

    public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResource>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        return identityResources.Where(identityResource => scopeNames.Contains(identityResource.Name));
    }

    public async Task<Resources> GetAllResourcesAsync()
    {
        var identityResources = await _memoryCacheClient.GetAsync<List<IdentityResource>>(CacheKeyConstants.IDENTITY_RESOURCE_KEY) ?? new();
        var apiScopes = await _memoryCacheClient.GetAsync<List<ApiScope>>(CacheKeyConstants.API_SCOPE_KEY) ?? new();
        var apiResources = await _memoryCacheClient.GetAsync<List<ApiResource>>(CacheKeyConstants.API_RESOURCE_KEY) ?? new();
        var resources = new Resources(identityResources, apiResources, apiScopes);
        return resources;
    }
}
