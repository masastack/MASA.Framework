// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Options;

public class OidcDbContextOptions
{
    internal IServiceProvider ServiceProvider { get; set; }

    public OidcDbContextOptions(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public async Task SeedClientDataAsync(List<Client> clients)
    {
        var clientRepository = ServiceProvider.GetRequiredService<IClientRepository>();

        foreach (var client in clients)
        {
            if (await clientRepository.FindAsync(s => s.ClientId == client.ClientId) is null)
            {
                await clientRepository.AddAsync(client);
            }
        }
    }

    public async Task SeedStandardResourcesAsync()
    {
        var userClaim = ServiceProvider.GetRequiredService<IUserClaimRepository>();
        var identityResourcerepository = ServiceProvider.GetRequiredService<IIdentityResourceRepository>();
        await userClaim.AddStandardUserClaimsAsync();
        await identityResourcerepository.AddStandardIdentityResourcesAsync();
    }

    public async Task SyncCacheAsync()
    {
        var syncCache = ServiceProvider.GetRequiredService<SyncCache>();
        await syncCache.ResetAsync();
    }
}
