// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ApiResourceRepository : IApiResourceRepository
{
    IApiResourceCache _cache;
    OidcDbContext _context;

    public ApiResourceRepository(IApiResourceCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<ApiResource>> GetPaginatedListAsync(int page, int pageSize)
    {
        var total = await _context.Set<ApiResource>().LongCountAsync();
        var apiResources = await _context.Set<ApiResource>()
                                               .OrderByDescending(s => s.ModificationTime)
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
                        .Include(apiResource => apiResource.Properties)
                        .Include(apiResource => apiResource.ApiScopes)
                        .FirstOrDefaultAsync(apiResource => apiResource.Id == id);

        return apiResource;
    }

    public async Task<List<ApiResource>> GetListAsync()
    {
        var apiResources = await _context.Set<ApiResource>().ToListAsync();
        return apiResources;
    }

    public async ValueTask<ApiResource> AddAsync(ApiResource apiResource)
    {
        var newApiResource = await _context.AddAsync(apiResource);
        await _cache.AddOrUpdateAsync(newApiResource.Entity);
        await UpdateCacheAsync();
        return apiResource;
    }

    public async Task<ApiResource> UpdateAsync(ApiResource apiResource)
    {
        var newApiResource = _context.Update(apiResource);
        await _cache.AddOrUpdateAsync(newApiResource.Entity);
        await UpdateCacheAsync();
        return apiResource;
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        _context.Remove(apiResource);        
        await _cache.RemoveAsync(apiResource);
        await UpdateCacheAsync();
    }

    private async Task UpdateCacheAsync()
    {
        var apiScopes = await GetListAsync();
        await _cache.AddAllAsync(apiScopes);
    }
}
