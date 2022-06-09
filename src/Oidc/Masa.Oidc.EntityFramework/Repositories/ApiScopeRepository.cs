// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ApiScopeRepository : IApiScopeRepository
{
    IApiScopeCache _cache;
    OidcDbContext _context;

    public ApiScopeRepository(IApiScopeCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<ApiScope>> GetPaginatedListAsync(int page, int pageSize)
    {
        var total = await _context.Set<ApiScope>().LongCountAsync();
        var apiScopes = await _context.Set<ApiScope>()
                                               .OrderByDescending(s => s.ModificationTime)
                                               .ThenByDescending(s => s.CreationTime)
                                               .Skip((page - 1) * pageSize)
                                               .Take(pageSize)
                                               .ToListAsync();
        return new PaginatedList<ApiScope>()
        {
            Total = total,
            Result = apiScopes
        };
    }

    public async Task<ApiScope?> GetDetailAsync(int id)
    {
        var apiScope = await _context.Set<ApiScope>()
                         .Include(apiScope => apiScope.UserClaims)
                         .Include(apiScope => apiScope.Properties)
                         .FirstOrDefaultAsync(apiScope => apiScope.Id == id);

        return apiScope;
    }

    public async Task<List<ApiScope>> GetListAsync()
    {
        var apiScopes = await _context.Set<ApiScope>().ToListAsync();
        return apiScopes;
    }

    public async ValueTask<ApiScope> AddAsync(ApiScope apiScope)
    {
        var newApiScope = await _context.AddAsync(apiScope);
        await _cache.AddOrUpdateAsync(newApiScope.Entity);
        await UpdateCacheAsync();
        return apiScope;
    }

    public async Task<ApiScope> UpdateAsync(ApiScope apiScope)
    {
        var newApiScope = _context.Update(apiScope);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(newApiScope.Entity);
        await UpdateCacheAsync();
        return apiScope;
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        _context.Remove(apiScope);
        await _cache.RemoveAsync(apiScope);
        await UpdateCacheAsync();
    }

    private async Task UpdateCacheAsync()
    {
        var apiScopes = await GetListAsync();
        await _cache.AddAllAsync(apiScopes);
    }
}
