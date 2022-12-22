// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Caches;

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

    public async Task SyncApiResourceCacheAsync(Guid id)
    {
        var apiResource = await ApiResourceQuery().FirstOrDefaultAsync(apiResource => apiResource.Id == id);
        if (apiResource is null) return;
        await _apiResourceCache.SetAsync(apiResource);
    }

    public async Task SyncApiScopeCacheAsync(Guid id)
    {
        var apiScope = await ApiScopeQuery().FirstOrDefaultAsync(apiScope => apiScope.Id == id);
        if (apiScope is null) return;
        await _apiScopeCache.SetAsync(apiScope);
    }

    public async Task SyncIdentityResourceCacheAsync(params Guid[] ids)
    {
        var identityResources = await IdentityResourceQuery().Where(idrs => ids.Contains(idrs.Id)).ToListAsync();
        if (identityResources.Count == 0) return;
        await _identityResourceCache.SetRangeAsync(identityResources);
    }

    public async Task RemoveApiResourceCacheAsync(ApiResource apiResource)
    {
        await _apiResourceCache.RemoveAsync(apiResource);
    }

    public async Task RemoveApiScopeCacheAsync(ApiScope apiScope)
    {
        await _apiScopeCache.RemoveAsync(apiScope);
    }

    public async Task RemoveIdentityResourceCacheAsync(IdentityResource identityResource)
    {
        await _identityResourceCache.RemoveAsync(identityResource);
    }

    public async Task ResetAsync()
    {
        var clients = await ClientQueryAsync();
        var apiScopes = await ApiScopeQuery().ToListAsync();
        var apiResources = await ApiResourceQuery().ToListAsync();
        var identityResource = await IdentityResourceQuery().ToListAsync();

        await _clientCache.ResetAsync(clients);
        await _apiScopeCache.ResetAsync(apiScopes);
        await _apiResourceCache.ResetAsync(apiResources);
        await _identityResourceCache.ResetAsync(identityResource);
    }

    private async Task<IEnumerable<Client>> ClientQueryAsync()
    {
        var clients = await _context.Set<Client>().ToListAsync();     
        var clientPropertys = await _context.Set<ClientProperty>().ToListAsync();
        var clientClaims = await _context.Set<ClientClaim>().ToListAsync();
        var clientIdPRestrictions = await _context.Set<ClientIdPRestriction>().ToListAsync();
        var clientCorsOrigins = await _context.Set<ClientCorsOrigin>().ToListAsync();
        var clientSecrets = await _context.Set<ClientSecret>().ToListAsync();
        var clientGrantTypes = await _context.Set<ClientGrantType>().ToListAsync();
        var clientRedirectUris = await _context.Set<ClientRedirectUri>().ToListAsync();
        var clientPostLogoutRedirectUris = await _context.Set<ClientPostLogoutRedirectUri>().ToListAsync();
        var clientScopes = await _context.Set<ClientScope>().ToListAsync();

        foreach(var client in clients)
        {
            client.AllowedGrantTypes.AddRange(clientGrantTypes.Where(item => item.ClientId == client.Id));
            client.RedirectUris.AddRange(clientRedirectUris.Where(item => item.ClientId == client.Id));
            client.PostLogoutRedirectUris.AddRange(clientPostLogoutRedirectUris.Where(item => item.ClientId == client.Id));
            client.Properties.AddRange(clientPropertys.Where(item => item.ClientId == client.Id));
            client.Claims.AddRange(clientClaims.Where(item => item.ClientId == client.Id));
            client.IdentityProviderRestrictions.AddRange(clientIdPRestrictions.Where(item => item.ClientId == client.Id));
            client.AllowedCorsOrigins.AddRange(clientCorsOrigins.Where(item => item.ClientId == client.Id));
            client.ClientSecrets.AddRange(clientSecrets.Where(item => item.ClientId == client.Id));
            client.AllowedScopes.AddRange(clientScopes.Where(item => item.ClientId == client.Id));
        }

        return clients;
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
