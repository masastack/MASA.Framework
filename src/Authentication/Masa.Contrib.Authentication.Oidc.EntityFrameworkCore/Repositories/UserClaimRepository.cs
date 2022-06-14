// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.Repositories;

public class UserClaimRepository : IUserClaimRepository
{
    OidcDbContext _context;

    public UserClaimRepository(OidcDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<UserClaim>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<UserClaim, bool>>? condition = null)
    {
        condition ??= userClaim => true;
        var query = _context.Set<UserClaim>().Where(condition);
        var total = await query.LongCountAsync();
        var userClaims = await query.OrderByDescending(s => s.ModificationTime)
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

    public async Task<UserClaim?> FindAsync(Expression<Func<UserClaim, bool>> predicate)
    {
        return await _context.Set<UserClaim>().FirstOrDefaultAsync(predicate);
    }

    public async Task<long> GetCountAsync(Expression<Func<UserClaim, bool>> predicate)
    {
        return await _context.Set<UserClaim>().Where(predicate).CountAsync();
    }

    public async ValueTask<UserClaim> AddAsync(UserClaim userClaim)
    {
        var exist = await _context.Set<UserClaim>().CountAsync(uc => uc.Name == userClaim.Name) > 0;
        if (exist)
            throw new UserFriendlyException($"UserClaim with name {userClaim.Name} already exists");

        var newUserClaim = await _context.AddAsync(userClaim);
        await _context.SaveChangesAsync();
        return newUserClaim.Entity;
    }

    public async Task<UserClaim> UpdateAsync(UserClaim userClaim)
    {
        var newUserClaim = _context.Update(userClaim);
        await _context.SaveChangesAsync();
        return newUserClaim.Entity;
    }

    public async Task RemoveAsync(UserClaim userClaim)
    {
        _context.Remove(userClaim);
        await _context.SaveChangesAsync();
    }

    public async Task AddStandardUserClaimsAsync()
    {
        var userClaims = new List<UserClaim>();
        foreach (var claim in StandardUserClaims.Claims)
        {
            var exist = await GetCountAsync(userClaim => userClaim.Name == claim.Key) > 0;
            if (exist) continue;

            userClaims.Add(new UserClaim(claim.Key, claim.Value));
        }
        await _context.AddRangeAsync(userClaims);
    }
}
