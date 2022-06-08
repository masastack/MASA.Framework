// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class IdentityResourceRepository : IIdentityResourceRepository
{
    IIdentityResourceCache _cache;   
    IRepository<IdentityResource> _repository;
    OidcDbContext _context;

    public IdentityResourceRepository(IIdentityResourceCache cache, IRepository<IdentityResource> repository, OidcDbContext context)
    {
        _cache = cache;
        _repository = repository;
        _context = context;
    }

    public async Task<PaginatedList<IdentityResource>> GetPaginatedListAsync(Expression<Func<IdentityResource, bool>> condition, PaginatedOptions options)
    {
        return await _repository.GetPaginatedListAsync(condition, options);
    }

    public async Task<IdentityResource?> GetDetailAsync(int id)
    {
        var identityResources = await _context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return identityResources;
    }

    public async Task<List<IdentityResource>> GetListAsync()
    {
        var identityResources = await _repository.GetListAsync();
        return identityResources.ToList();
    }

    public async ValueTask<IdentityResource> AddAsync(IdentityResource identityResource)
    {
        var newIdentityResource = await _repository.AddAsync(identityResource);
        await _cache.AddOrUpdateAsync(newIdentityResource);
        return identityResource;
    }

    public async Task<IdentityResource> UpdateAsync(IdentityResource identityResource)
    {
        var newIdentityResource = await _repository.UpdateAsync(identityResource);
        await _cache.AddOrUpdateAsync(newIdentityResource);
        return identityResource;
    }

    public async Task RemoveAsync(IdentityResource identityResource)
    {
        await _repository.RemoveAsync(identityResource);
        await _cache.RemoveAsync(identityResource);
    }
}
