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

    public static async Task AddOidcDbContext<T>(this IServiceCollection services, Func<OidcDbContextOptions, Task> options) where T : DbContext
    {
        services.AddOidcDbContext<T>();
        await options.Invoke(new OidcDbContextOptions(services));
    }
}
