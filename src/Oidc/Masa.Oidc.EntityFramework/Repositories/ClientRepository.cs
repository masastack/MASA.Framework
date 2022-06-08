// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.Repositories;

public class ClientRepository : IClientRepository
{
    IClientCache _cache;
    IRepository<Client> _repository;
    OidcDbContext _context;

    public ClientRepository(IClientCache cache, IRepository<Client> repository, OidcDbContext context)
    {
        _cache = cache;
        _repository = repository;
        _context = context;
    }

    public async Task<PaginatedList<Client>> GetPaginatedListAsync(Expression<Func<Client, bool>> condition, PaginatedOptions options)
    {
        return await _repository.GetPaginatedListAsync(condition, options);
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
        var clients = await _repository.GetListAsync();
        return clients.ToList();
    }

    public async ValueTask<Client> AddAsync(Client client)
    {
        var newClient = await _repository.AddAsync(client);
        await _cache.AddOrUpdateAsync(newClient);
        return newClient;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        var newClient = await _repository.UpdateAsync(client);
        await _cache.AddOrUpdateAsync(newClient);
        return newClient;
    }

    public async Task RemoveAsync(Client client)
    {
        await _repository.RemoveAsync(client);
        await _cache.RemoveAsync(client);
    }
}
