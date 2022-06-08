// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ApiScopeRepository : IApiScopeRepository
{
    IApiScopeCache _cache;
    IRepository<ApiScope> _repository;
    OidcDbContext _context;

    public ApiScopeRepository(IApiScopeCache cache, IRepository<ApiScope> repository, OidcDbContext context)
    {
        _cache = cache;
        _repository = repository;
        _context = context;
    }

    public async Task<PaginatedList<ApiScope>> GetPaginatedListAsync(Expression<Func<ApiScope, bool>> condition, PaginatedOptions options)
    {
        return await _repository.GetPaginatedListAsync(condition, options);
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
        var apiScopes = await _repository.GetListAsync();
        return apiScopes.ToList();
    }

    public async ValueTask<ApiScope> AddAsync(ApiScope apiScope)
    {
        var newApiScope = await _repository.AddAsync(apiScope);
        await _cache.AddOrUpdateAsync(newApiScope);
        return apiScope;
    }

    public async Task<ApiScope> UpdateAsync(ApiScope apiScope)
    {
        var newApiScope = await _repository.UpdateAsync(apiScope);
        await _cache.AddOrUpdateAsync(newApiScope);
        return apiScope;
    }

    public async Task RemoveAsync(ApiScope apiScope)
    {
        await _repository.RemoveAsync(apiScope);
        await _cache.RemoveAsync(apiScope);
    }
}
