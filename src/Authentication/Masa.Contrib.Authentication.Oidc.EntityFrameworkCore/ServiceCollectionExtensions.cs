// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        services.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<T>()));
        services.AddScoped<IUserClaimRepository, UserClaimRepository>();
        services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IApiScopeRepository, ApiScopeRepository>();
        services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
        services.AddScoped<SyncCache>();

        return services;
    }

    public static IServiceCollection SeedClientData(this IServiceCollection services, List<Client> clients)
    {
        var clientRepository = services.BuildServiceProvider().GetRequiredService<IClientRepository>();
        var apiScopeRepository = services.BuildServiceProvider().GetRequiredService<IApiScopeRepository>();

        var scopes = clients.SelectMany(c => c.AllowedScopes);
        foreach (var scope in scopes)
        {
            if (apiScopeRepository.FindAsync(s => s.Name == scope.Scope).Result == null)
            {
                _ = apiScopeRepository.AddAsync(new ApiScope(scope.Scope)).Result;
            }
        }
        foreach (var client in clients)
        {
            if (clientRepository.FindAsync(s => s.ClientId == client.ClientId).Result == null)
            {
                _ = clientRepository.AddAsync(client).Result;
            }
        }
        return services;
    }
}
