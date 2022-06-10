// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.Cache.Storage.Stores;

public class ResourceStore : IResourceStore
{
    IIdentityResourceCache _identityResourceCache;
    IApiResourceCache _apiResourceCache;
    IApiScopeCache _apiScopeCache;

    public ResourceStore(IIdentityResourceCache identityResourceCache, IApiResourceCache apiResourceCache, IApiScopeCache apiScopeCache)
    {
        _identityResourceCache = identityResourceCache;
        _apiResourceCache = apiResourceCache;
        _apiScopeCache = apiScopeCache;
    }

    public async Task<IEnumerable<ApiResourceModel>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        ArgumentNullException.ThrowIfNull(apiResourceNames);

        var apiResources = await _apiResourceCache.GetListAsync(apiResourceNames);
        return apiResources;
    }

    public async Task<IEnumerable<ApiResourceModel>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var apiResources = await _apiResourceCache.GetListAsync();
        return apiResources.Where(apiResource => apiResource.Scopes?.Any(scope => scopeNames.Contains(scope)) is true);
    }

    public async Task<IEnumerable<ApiScopeModel>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var apiScopes = await _apiScopeCache.GetListAsync(scopeNames);
        return apiScopes;
    }

    public async Task<IEnumerable<IdentityResourceModel>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        ArgumentNullException.ThrowIfNull(scopeNames);

        var identityResources = await _identityResourceCache.GetListAsync(scopeNames);
        return identityResources;
    }

    public async Task<ResourcesModel> GetAllResourcesAsync()
    {
        var identityResources = await _identityResourceCache.GetListAsync();
        var apiScopes = await _apiScopeCache.GetListAsync();
        var apiResources = await _apiResourceCache.GetListAsync();
        var resources = new ResourcesModel(identityResources, apiResources, apiScopes);
        return resources;
    }
}
