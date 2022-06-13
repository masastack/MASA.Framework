// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Oidc.EntityFramework.Repositories;

public class ClientRepository : IClientRepository
{
    IClientCache _cache;
    OidcDbContext _context;

    public ClientRepository(IClientCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<Client>> GetPaginatedListAsync(int page, int pageSize, Expression<Func<Client, bool>>? condition = null)
    {
        condition ??= userClaim => true;
        var query = _context.Set<Client>().Where(condition);
        var total = await query.LongCountAsync();
        var clients = await query.OrderByDescending(s => s.ModificationTime)
                                .ThenByDescending(s => s.CreationTime)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
        return new PaginatedList<Client>()
        {
            Total = total,
            Result = clients
        };
    }

    public async Task<Client?> GetDetailAsync(int id)
    {
        return await _context.Set<Client>()
                    .Where(c => c.Id == id)
                    .Include(c => c.AllowedGrantTypes)
                    .Include(c => c.RedirectUris)
                    .Include(c => c.PostLogoutRedirectUris)
                    .Include(c => c.Properties)
                    .Include(c => c.Claims)
                    .Include(c => c.IdentityProviderRestrictions)
                    .Include(c => c.AllowedCorsOrigins)
                    .Include(c => c.ClientSecrets)
                    .Include(c => c.AllowedScopes)
                    .FirstOrDefaultAsync();
    }

    public async Task<List<Client>> GetListAsync()
    {
        var clients = await _context.Set<Client>().ToListAsync();
        return clients;
    }

    public async Task<Client?> FindAsync(Expression<Func<Client, bool>> predicate)
    {
        return await _context.Set<Client>().FirstOrDefaultAsync(predicate);
    }

    public async Task<long> GetCountAsync(Expression<Func<Client, bool>> predicate)
    {
        return await _context.Set<Client>().Where(predicate).CountAsync();
    }

    public async ValueTask<Client> AddAsync(Client client)
    {
        var newClient = await _context.AddAsync(client);
        await _context.SaveChangesAsync();
        var detail = await GetDetailAsync(newClient.Entity.Id);
        await _cache.SetAsync(detail!);
        return newClient.Entity;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        var newClient = _context.Update(client);
        await _context.SaveChangesAsync();
        var detail = await GetDetailAsync(client.Id);
        await _cache.SetAsync(detail!);
        return newClient.Entity;
    }

    public async Task RemoveAsync(Client client)
    {
        _context.Remove(client);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(client);
    }
}
