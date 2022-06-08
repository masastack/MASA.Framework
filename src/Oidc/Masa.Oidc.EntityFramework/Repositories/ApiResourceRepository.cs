// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ApiResourceRepository : IApiResourceRepository
{
    IApiResourceCache _cache;
    IRepository<ApiResource> _repository;
    OidcDbContext _context;

    public ApiResourceRepository(IApiResourceCache cache, IRepository<ApiResource> repository, OidcDbContext context)
    {
        _cache = cache;
        _repository = repository;
        _context = context;
    }

    public async Task<PaginatedList<ApiResource>> GetPaginatedListAsync(Expression<Func<ApiResource, bool>> condition, PaginatedOptions options)
    {
        return await _repository.GetPaginatedListAsync(condition, options);
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
        var apiResources = await _repository.GetListAsync();
        return apiResources.ToList();
    }

    public async ValueTask<ApiResource> AddAsync(ApiResource apiResource)
    {
        var newApiResource = await _repository.AddAsync(apiResource);
        await _cache.AddOrUpdateAsync(newApiResource);
        return apiResource;
    }

    public async Task<ApiResource> UpdateAsync(ApiResource apiResource)
    {
        var newApiResource = await _repository.UpdateAsync(apiResource);
        await _cache.AddOrUpdateAsync(newApiResource);
        return apiResource;
    }

    public async Task RemoveAsync(ApiResource apiResource)
    {
        await _repository.RemoveAsync(apiResource);
        await _cache.RemoveAsync(apiResource);
    }
}
