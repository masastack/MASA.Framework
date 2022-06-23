// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.Repositories;

public class ApiResourceRepository : IApiResourceRepository
{
    SyncCache _cache;
    OidcDbContext _context;

    public ApiResourceRepository(SyncCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<ApiResource>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<ApiResource, bool>>? condition = null)
    {
        condition ??= userClaim => true;
        var query = _context.Set<ApiResource>().Where(condition);
        var total = await query.LongCountAsync();
        var apiResources = await query.OrderByDescending(s => s.ModificationTime)
                                    .ThenByDescending(s => s.CreationTime)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
        return new PaginatedList<ApiResource>()
        {
            Total = total,
            Result = apiResources
        };
    }

    public async Task<ApiResource?> GetDetailAsync(int id)
    {
        var apiResource = await _context.Set<ApiResource>()
                        .Include(apiResource => apiResource.UserClaims)
                        .ThenInclude(userClaim => userClaim.UserClaim)
                        .Include(apiResource => apiResource.Properties)
                        .Include(apiResource => apiResource.ApiScopes)
                        .ThenInclude(apiScope => apiScope.ApiScope)
                        .FirstOrDefaultAsync(apiResource => apiResource.Id == id);

        return apiResource;
    }

    public async Task<List<ApiResource>> GetListAsync()
    {
        var apiResources = await _context.Set<ApiResource>().ToListAsync();
        return apiResources;
    }

    public async Task<ApiResource?> FindAsync(Expression<Func<ApiResource, bool>> predicate)
    {
        return await _context.Set<ApiResource>().FirstOrDefaultAsync(predicate);
    }

    public async Task<long> GetCountAsync(Expression<Func<ApiResource, bool>> predicate)
    {
        return await _context.Set<ApiResource>().Where(predicate).CountAsync();
    }

    public async ValueTask<ApiResource> AddAsync(ApiResource apiResource)
    {
        var exist = await _context.Set<ApiResource>().CountAsync(a => a.Name == apiResource.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ApiResource with name {apiResource.Name} already exists");

        var newApiResource = await _context.AddAsync(apiResource);
        await _context.SaveChangesAsync();
        await _cache.SyncApiResourceCacheAsync(apiResource.Id);
        return newApiResource.Entity;
    }

    public async Task<ApiResource> UpdateAsync(ApiResource apiResource)
    {
        var newApiResource = _context.Update(apiResource);
        await _context.SaveChangesAsync();
        await _cache.SyncApiResourceCacheAsync(apiResource.Id);
        return newApiResource.Entity;
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        _context.Remove(apiResource);
        await _context.SaveChangesAsync();
        await _cache.RemoveApiResourceCacheAsync(apiResource);
    }
}
