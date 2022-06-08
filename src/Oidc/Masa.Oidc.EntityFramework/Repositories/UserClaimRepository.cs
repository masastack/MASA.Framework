// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class UserClaimRepository : IUserClaimRepository
{
    IRepository<UserClaim> _repository;
    OidcDbContext _context;

    public UserClaimRepository(IRepository<UserClaim> repository, OidcDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<PaginatedList<UserClaim>> GetPaginatedListAsync(Expression<Func<UserClaim, bool>> condition, PaginatedOptions options)
    {
        return await _repository.GetPaginatedListAsync(condition, options);
    }

    public async Task<UserClaim?> GetDetailAsync(int id)
    {
        var userClaim = await _context.Set<UserClaim>()
                         .FirstOrDefaultAsync(userClaim => userClaim.Id == id);

        return userClaim;
    }

    public async Task<List<UserClaim>> GetListAsync()
    {
        var userClaims = await _repository.GetListAsync();
        return userClaims.ToList();
    }

    public async ValueTask<UserClaim> AddAsync(UserClaim userClaim)
    {
        var newUserClaim = await _repository.AddAsync(userClaim);
        return userClaim;
    }

    public async Task<UserClaim> UpdateAsync(UserClaim userClaim)
    {
        var newUserClaim = await _repository.UpdateAsync(userClaim);
        return userClaim;
    }

    public async Task RemoveAsync(UserClaim userClaim)
    {
        await _repository.RemoveAsync(userClaim);
    }
}
