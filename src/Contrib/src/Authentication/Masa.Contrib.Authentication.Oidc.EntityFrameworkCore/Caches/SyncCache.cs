// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.Caches;

public class SyncCache
{
    IClientCache _clientCache;
    IApiResourceCache _apiResourceCache;
    IApiScopeCache _apiScopeCache;
    IIdentityResourceCache _identityResourceCache;
    DbContext _context;

    public SyncCache(IClientCache clientCache, IApiResourceCache apiResourceCache, IApiScopeCache apiScopeCache, IIdentityResourceCache identityResourceCache, OidcDbContext context)
    {
        _clientCache = clientCache;
        _apiResourceCache = apiResourceCache;
        _apiScopeCache = apiScopeCache;
        _identityResourceCache = identityResourceCache;
        _context = context;
    }

    internal async Task SyncApiResourceCacheAsync(int id)
    {
        var apiResource = await ApiResourceQuery().FirstOrDefaultAsync(apiResource => apiResource.Id == id);
        if (apiResource is null) return;
        await _apiResourceCache.SetAsync(apiResource);
    }

    internal async Task SyncApiScopeCacheAsync(int id)
    {
        var apiScope = await ApiScopeQuery().FirstOrDefaultAsync(apiScope => apiScope.Id == id);
        if (apiScope is null) return;
        await _apiScopeCache.SetAsync(apiScope);
    }

    internal async Task SyncIdentityResourceCacheAsync(params int[] ids)
    {
        var identityResources = await IdentityResourceQuery().Where(idrs => ids.Contains(idrs.Id)).ToListAsync();
        if (identityResources.Count < 0) return;
        await _identityResourceCache.SetRangeAsync(identityResources);
    }

    internal async Task RemoveApiResourceCacheAsync(ApiResource apiResource)
    {
        await _apiResourceCache.RemoveAsync(apiResource);
    }

    internal async Task RemoveApiScopeCacheAsync(ApiScope apiScope)
    {
        await _apiScopeCache.RemoveAsync(apiScope);
    }

    internal async Task RemoveIdentityResourceCacheAsync(IdentityResource identityResource)
    {
        await _identityResourceCache.RemoveAsync(identityResource);
    }

    public async Task ResetAsync()
    {
        var clients = await ClientQuery().ToListAsync();
        var apiScopes = await ApiScopeQuery().ToListAsync();
        var apiResources = await ApiResourceQuery().ToListAsync();
        var identityResource = await IdentityResourceQuery().ToListAsync();

        await _clientCache.ResetAsync(clients);
        await _apiScopeCache.ResetAsync(apiScopes);
        await _apiResourceCache.ResetAsync(apiResources);
        await _identityResourceCache.ResetAsync(identityResource);
    }

    private IQueryable<Client> ClientQuery()
    {
        return _context.Set<Client>()
                    .Include(c => c.AllowedGrantTypes)
                    .Include(c => c.RedirectUris)
                    .Include(c => c.PostLogoutRedirectUris)
                    .Include(c => c.Properties)
                    .Include(c => c.Claims)
                    .Include(c => c.IdentityProviderRestrictions)
                    .Include(c => c.AllowedCorsOrigins)
                    .Include(c => c.ClientSecrets)
                    .Include(c => c.AllowedScopes);
    }

    private IQueryable<IdentityResource> IdentityResourceQuery()
    {
        return _context.Set<IdentityResource>()
                    .Include(idrs => idrs.UserClaims)
                    .ThenInclude(uc => uc.UserClaim)
                    .Include(idrs => idrs.Properties);
    }

    private IQueryable<ApiScope> ApiScopeQuery()
    {
        return _context.Set<ApiScope>()
                    .Include(apiScope => apiScope.UserClaims)
                    .ThenInclude(apiScope => apiScope.UserClaim)
                    .Include(apiScope => apiScope.Properties);
    }

    private IQueryable<ApiResource> ApiResourceQuery()
    {
        return _context.Set<ApiResource>()
                    .Include(apiResource => apiResource.UserClaims)
                    .ThenInclude(userClaim => userClaim.UserClaim)
                    .Include(apiResource => apiResource.Properties)
                    .Include(apiResource => apiResource.ApiScopes)
                    .ThenInclude(apiScope => apiScope.ApiScope);
    }
}
