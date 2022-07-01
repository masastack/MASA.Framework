// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.Options;

public class OidcDbContextOptions
{
    IServiceProvider ServiceProvider { get; set; }

    public OidcDbContextOptions(IServiceCollection services)
    {
        ServiceProvider = services.BuildServiceProvider();
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
        var repository = ServiceProvider.GetRequiredService<IIdentityResourceRepository>();
        await repository.AddStandardIdentityResourcesAsync();
    }
}
