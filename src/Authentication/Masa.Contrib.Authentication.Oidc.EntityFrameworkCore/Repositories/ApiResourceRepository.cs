// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.Repositories;

public class ApiResourceRepository : IApiResourceRepository
{
    SyncCache _cache;
    DbContext _context;
    IRepository<ApiResource> _repository;

    public ApiResourceRepository(SyncCache cache, OidcDbContext context, IRepository<ApiResource> repository)
    {
        _cache = cache;
        _context = context;
        _repository = repository;
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

        var newApiResource = await _repository.AddAsync(apiResource);
        await _cache.SyncApiResourceCacheAsync(apiResource.Id);
        return newApiResource;
    }

    public async Task<ApiResource> UpdateAsync(ApiResource apiResource)
    {
        var newApiResource = await _repository.UpdateAsync(apiResource);
        await _cache.SyncApiResourceCacheAsync(apiResource.Id);
        return newApiResource;
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        await _repository.RemoveAsync(apiResource);
        await _cache.RemoveApiResourceCacheAsync(apiResource);
    }
}
