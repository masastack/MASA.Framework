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

    public async Task<PaginatedList<IdentityResource>> GetPaginatedListAsync(int page, int pageSize)
    {
        var total = await _context.Set<IdentityResource>().LongCountAsync();
        var identityResources = await _context.Set<IdentityResource>()
                                               .OrderByDescending(s => s.ModificationTime)
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
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return identityResources;
    }

    public async ValueTask<IdentityResource> AddAsync(IdentityResource identityResource)
    {
        var newIdentityResource = await _context.AddAsync(identityResource);
        await _cache.AddOrUpdateAsync(newIdentityResource.Entity);
        await UpdateCacheAsync();
        return newIdentityResource.Entity;
    }

    public async Task<IdentityResource> UpdateAsync(IdentityResource identityResource)
    {
        var newIdentityResource = _context.Update(identityResource);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(newIdentityResource.Entity);
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
}
