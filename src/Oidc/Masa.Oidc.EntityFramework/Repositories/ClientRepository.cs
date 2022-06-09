// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ClientRepository : IClientRepository
{
    IClientCache _cache;
    OidcDbContext _context;

    public ClientRepository(IClientCache cache, OidcDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<PaginatedList<Client>> GetPaginatedListAsync(int page, int pageSize)
    {
        var total = await _context.Set<Client>().LongCountAsync();
        var clients = await _context.Set<Client>()
                                               .OrderByDescending(s => s.ModificationTime)
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

    public async ValueTask<Client> AddAsync(Client client)
    {
        var newClient = await _context.AddAsync(client);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(newClient.Entity);
        return newClient.Entity;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        var newClient = _context.Update(client);
        await _context.SaveChangesAsync();
        await _cache.AddOrUpdateAsync(newClient.Entity);
        return newClient.Entity;
    }

    public async Task RemoveAsync(Client client)
    {
        _context.Remove(client);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync(client);
    }
}
