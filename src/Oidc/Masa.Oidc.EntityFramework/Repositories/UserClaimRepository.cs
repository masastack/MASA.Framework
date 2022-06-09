// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class UserClaimRepository : IUserClaimRepository
{
    OidcDbContext _context;

    public UserClaimRepository(OidcDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<UserClaim>> GetPaginatedListAsync(int page, int pageSize)
    {
        var total = await _context.Set<UserClaim>().LongCountAsync();
        var userClaims = await _context.Set<UserClaim>()
                                               .OrderByDescending(s => s.ModificationTime)
                                               .ThenByDescending(s => s.CreationTime)
                                               .Skip((page - 1) * pageSize)
                                               .Take(pageSize)
                                               .ToListAsync();
        return new PaginatedList<UserClaim>()
        {
            Total = total,
            Result = userClaims
        };
    }

    public async Task<UserClaim?> GetDetailAsync(int id)
    {
        var userClaim = await _context.Set<UserClaim>()
                         .FirstOrDefaultAsync(userClaim => userClaim.Id == id);

        return userClaim;
    }

    public async Task<List<UserClaim>> GetListAsync()
    {
        var userClaims = await _context.Set<UserClaim>().ToListAsync();
        return userClaims;
    }

    public async ValueTask<UserClaim> AddAsync(UserClaim userClaim)
    {
        var newUserClaim = await _context.AddAsync(userClaim);
        return newUserClaim.Entity;
    }

    public async Task<UserClaim> UpdateAsync(UserClaim userClaim)
    {
        var newUserClaim = _context.Update(userClaim);
        await _context.SaveChangesAsync();
        return userClaim;
    }

    public async Task RemoveAsync(UserClaim userClaim)
    {
        _context.Remove(userClaim);
        await _context.SaveChangesAsync();
    }
}
