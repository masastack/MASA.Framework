// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EntityFrameworkCore.Repositories;

public class IdentityResourceRepository : IIdentityResourceRepository
{
    SyncCache _cache;
    DbContext _context;
    IRepository<IdentityResource> _repository;

    public IdentityResourceRepository(SyncCache cache, OidcDbContext context, IRepository<IdentityResource> repository)
    {
        _cache = cache;
        _context = context;
        _repository = repository;
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
        var exist = await _context.Set<IdentityResource>().CountAsync(idrs => idrs.Name == identityResource.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"IdentityResource with name {identityResource.Name} already exists");

        var newIdentityResource = await _repository.AddAsync(identityResource);
        await _context.SaveChangesAsync();
        await _cache.SyncIdentityResourceCacheAsync(identityResource.Id);
        return newIdentityResource;
    }

    public async Task<IdentityResource> UpdateAsync(IdentityResource identityResource)
    {
        var newIdentityResource = await _repository.UpdateAsync(identityResource);
        await _context.SaveChangesAsync();
        await _cache.SyncIdentityResourceCacheAsync(identityResource.Id);

        return newIdentityResource;
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        await _repository.RemoveAsync(identityResource);
        await _context.SaveChangesAsync();
        await _cache.RemoveIdentityResourceCacheAsync(identityResource);
    }

    public async Task AddStandardIdentityResourcesAsync()
    {
        var userClaims = await _context.Set<UserClaim>().ToListAsync();
        var syncIdentityResources = new List<IdentityResource>();
        foreach (var identityResource in StandardIdentityResources.IdentityResources)
        {
            var userClaimIds = userClaims.Where(uc => identityResource.UserClaims.Contains(uc.Name)).Select(uc => uc.Id);
            var existData = await FindAsync(idrs => idrs.Name == identityResource.Name);
            if (existData is not null)
            {
                existData.Update(identityResource.DisplayName, identityResource.Description ?? "", true, identityResource.Required, identityResource.Emphasize, identityResource.ShowInDiscoveryDocument, true);
                existData.BindUserClaims(userClaimIds);
                await _repository.UpdateAsync(existData);
            }
            else
            {
                existData = new IdentityResource(identityResource.Name, identityResource.DisplayName, identityResource.Description ?? "", true, identityResource.Required, identityResource.Enabled, identityResource.ShowInDiscoveryDocument, true);
                existData.BindUserClaims(userClaimIds);
                await _repository.AddAsync(existData);
            }
            syncIdentityResources.Add(existData);
        }
        await _context.SaveChangesAsync();
        await _cache.SyncIdentityResourceCacheAsync(syncIdentityResources.Select(idrs => idrs.Id).ToArray());
    }
}
