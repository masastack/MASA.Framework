// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Repositories;

public class ApiScopeRepository : IApiScopeRepository
{
    SyncCache _cache;
    DbContext _context;
    IRepository<ApiScope> _repository;

    public ApiScopeRepository(SyncCache cache, OidcDbContext context, IRepository<ApiScope> repository)
    {
        _cache = cache;
        _context = context;
        _repository = repository;
    }

    public async Task<PaginatedList<ApiScope>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<ApiScope, bool>>? condition = null)
    {
        condition ??= userClaim => true;
        var query = _context.Set<ApiScope>().Where(condition);
        var total = await query.LongCountAsync();
        var apiScopes = await query.OrderByDescending(s => s.ModificationTime)
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
                         .ThenInclude(apiScope => apiScope.UserClaim)
                         .Include(apiScope => apiScope.Properties)
                         .AsSplitQuery()
                         .FirstOrDefaultAsync(apiScope => apiScope.Id == id);

        return apiScope;
    }

    public async Task<List<ApiScope>> GetListAsync()
    {
        var apiScopes = await _context.Set<ApiScope>().ToListAsync();
        return apiScopes;
    }

    public async Task<ApiScope?> FindAsync(Expression<Func<ApiScope, bool>> predicate)
    {
        return await _context.Set<ApiScope>().FirstOrDefaultAsync(predicate);
    }

    public async Task<long> GetCountAsync(Expression<Func<ApiScope, bool>> predicate)
    {
        return await _context.Set<ApiScope>().Where(predicate).CountAsync();
    }

    public async ValueTask<ApiScope> AddAsync(ApiScope apiScope)
    {
        var exist = await _context.Set<ApiScope>().CountAsync(a => a.Name == apiScope.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"ApiScope with name {apiScope.Name} already exists");

        var newApiScope = await _repository.AddAsync(apiScope);
        await _context.SaveChangesAsync();
        await _cache.SyncApiScopeCacheAsync(apiScope.Id);
        return newApiScope;
    }

    public async Task<ApiScope> UpdateAsync(ApiScope apiScope)
    {
        var newApiScope = await _repository.UpdateAsync(apiScope);
        await _context.SaveChangesAsync();
        await _cache.SyncApiScopeCacheAsync(apiScope.Id);
        return newApiScope;
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        await _repository.RemoveAsync(apiScope);
        await _context.SaveChangesAsync();
        await _cache.RemoveApiScopeCacheAsync(apiScope);
    }
}
