// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class IdentityResourceRepository : IIdentityResourceRepository
{
    IIdentityResourceCache _cache;
    OidcDbContext _context;

    public IdentityResourceRepository(IIdentityResourceCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<IdentityResource>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<IdentityResource, bool>>? condition = null)
    {
        condition ??= userClaim => true;
        var query = _context.Set<IdentityResource>().Where(condition);
        var total = await query.LongCountAsync();
        var identityResources = await query.OrderByDescending(s => s.ModificationTime)
                                            .ThenByDescending(s => s.CreationTime)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();
        return new PaginatedList<IdentityResource>()
        {
            Total = total,
            Result = identityResources
        };
    }

    public async Task<List<IdentityResource>> GetListAsync()
    {
        return await _context.Set<IdentityResource>().ToListAsync();
    }

    public async Task<IdentityResource?> GetDetailAsync(int id)
    {
        var identityResources = await _context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .ThenInclude(uc => uc.UserClaim)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return identityResources;
    }

    public async Task<IdentityResource?> FindAsync(Expression<Func<IdentityResource, bool>> predicate)
    {
        return await _context.Set<IdentityResource>().FirstOrDefaultAsync(predicate);
    }

    public async Task<long> GetCountAsync(Expression<Func<IdentityResource, bool>> predicate)
    {
        return await _context.Set<IdentityResource>().Where(predicate).CountAsync();
    }

    public async ValueTask<IdentityResource> AddAsync(IdentityResource identityResource)
    {
        var newIdentityResource = await _context.AddAsync(identityResource);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(await GetDetailAsync(identityResource.Id));
        await UpdateCacheAsync();
        return newIdentityResource.Entity;
    }

    public async Task<IdentityResource> UpdateAsync(IdentityResource identityResource)
    {
        var newIdentityResource = _context.Update(identityResource);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(await GetDetailAsync(identityResource.Id));
        await UpdateCacheAsync();

        return identityResource;
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        _context.Remove(identityResource);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(identityResource);
        await UpdateCacheAsync();
    }

    private async Task UpdateCacheAsync()
    {
        var identityResources = await GetListAsync();
        await _cache.AddAllAsync(identityResources);
    }

    public async Task AddStandardIdentityResourcesAsync()
    {
        var userClaims = await _context.Set<UserClaim>().ToListAsync();
        foreach (var identityResource in StandardIdentityResources.IdentityResources)
        {
            var userClaimIds = userClaims.Where(uc => identityResource.UserClaims.Contains(uc.Name)).Select(uc => uc.Id);
            var existData = await FindAsync(idrs => idrs.Name == identityResource.Name);
            if (existData is not null)
            {
                existData.Update(identityResource.DisplayName, identityResource.Description ?? "", true, identityResource.Required, identityResource.Emphasize, identityResource.ShowInDiscoveryDocument, true);
                existData.BindUserClaims(userClaimIds);
                await UpdateAsync(existData);
            }
            else
            {
                var idrs = new IdentityResource(identityResource.Name, identityResource.DisplayName, identityResource.Description ?? "", true, identityResource.Required, identityResource.Enabled, identityResource.ShowInDiscoveryDocument, true);
                idrs.BindUserClaims(userClaimIds);
                await AddAsync(idrs);
            }
        }
    }
}
