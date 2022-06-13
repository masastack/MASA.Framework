// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.Caches;

public class SyncCache
{
    IClientCache _clientCache;
    IApiResourceCache _apiResourceCache;
    IApiScopeCache _apiScopeCache;
    IIdentityResourceCache _identityResourceCache;
    OidcDbContext _context;

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
        await SyncAllApiResourceCacheAsync();
    }

    internal async Task SyncApiScopeCacheAsync(int id)
    {
        var apiScope = await ApiScopeQuery().FirstOrDefaultAsync(apiScope => apiScope.Id == id);
        if (apiScope is null) return;
        await _apiScopeCache.SetAsync(apiScope);
        await SyncAllApiScopeCacheAsync();
    }

    internal async Task SyncIdentityResourceCacheAsync(params int[] ids)
    {
        var identityResources = await IdentityResourceQuery().Where(idrs => ids.Contains(idrs.Id)).ToListAsync();
        if (identityResources.Count < 0) return;
        await _identityResourceCache.SetRangeAsync(identityResources);
        await SyncAllIdentityResourceAsync();
    }

    internal async Task RemoveApiResourceCacheAsync(ApiResource apiResource)
    {
        await _apiResourceCache.RemoveAsync(apiResource);
        await SyncAllApiResourceCacheAsync();
    }

    internal async Task RemoveApiScopeCacheAsync(ApiScope apiScope)
    {
        await _apiScopeCache.RemoveAsync(apiScope);
        await SyncAllApiScopeCacheAsync();
    }

    internal async Task RemoveIdentityResourceCacheAsync(IdentityResource identityResource)
    {
        await _identityResourceCache.RemoveAsync(identityResource);
        await SyncAllIdentityResourceAsync();
    }

    internal async Task<List<ApiResource>> SyncAllApiResourceCacheAsync()
    {
        var apiResources = await ApiResourceQuery().AsSplitQuery().ToListAsync();
        await _apiResourceCache.AddAllAsync(apiResources);

        return apiResources;
    }

    internal async Task<List<ApiScope>> SyncAllApiScopeCacheAsync()
    {
        var apiScopes = await ApiScopeQuery().AsSplitQuery().ToListAsync();
        await _apiScopeCache.AddAllAsync(apiScopes);
        return apiScopes;
    }

    internal async Task<List<IdentityResource>> SyncAllIdentityResourceAsync()
    {
        var identityResources = await IdentityResourceQuery().AsSplitQuery().ToListAsync();
        await _identityResourceCache.AddAllAsync(identityResources);
        return identityResources;
    }

    public async Task InitializeAsync()
    {
        var apiResources = await SyncAllApiResourceCacheAsync();
        var apiScopes = await SyncAllApiScopeCacheAsync();
        var identityResources = await SyncAllIdentityResourceAsync();

        await _apiResourceCache.SetRangeAsync(apiResources);
        await _apiScopeCache.SetRangeAsync(apiScopes);
        await _identityResourceCache.SetRangeAsync(identityResources);

        var clients = await ClientQuery().ToListAsync();
        await _clientCache.SetRangeAsync(clients);
    }

    internal async Task RefreshCacheWithRemoveUserClaimAsync(int userClaimId)
    {
        var apiResources = await ApiResourceQuery().Where(apiResource => apiResource.UserClaims.Any(userClaim => userClaim.UserClaimId == userClaimId))
                                                  .AsSplitQuery()
                                                  .ToListAsync();
        var apiScopes = await ApiScopeQuery().Where(apiScope => apiScope.UserClaims.Any(userClaim => userClaim.UserClaimId == userClaimId))
                                            .AsSplitQuery()
                                            .ToListAsync();
        var identityResources = await IdentityResourceQuery().Where(identityResource => identityResource.UserClaims.Any(userClaim => userClaim.UserClaimId == userClaimId))
                                            .AsSplitQuery()
                                            .ToListAsync();
        if (apiResources.Count > 0)
        {
            await _apiResourceCache.SetRangeAsync(apiResources);
            await SyncAllApiResourceCacheAsync();
        }
        if (apiScopes.Count > 0)
        {
            await _apiScopeCache.SetRangeAsync(apiScopes);
            await SyncAllApiResourceCacheAsync();
        }
        if (identityResources.Count > 0)
        {
            await _identityResourceCache.SetRangeAsync(identityResources);
            await SyncAllApiResourceCacheAsync();
        }
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
