// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcDbContext(this IServiceCollection services,Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<OidcDbContext>(optionsAction);
        services.AddScoped<IUserClaimRepository, UserClaimRepository>();
        services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IApiScopeRepository, ApiScopeRepository>();
        services.AddScoped<IApiResourceRepository, ApiResourceRepository>();

        return services;
    }
}
