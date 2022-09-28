// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcDbContext<T>(this IServiceCollection services) where T : DbContext, IMasaDbContext
    {
        services.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<T>()));
        services.AddScoped<IUserClaimRepository, UserClaimRepository>();
        services.AddScoped<IIdentityResourceRepository, IdentityResourceRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IApiScopeRepository, ApiScopeRepository>();
        services.AddScoped<IApiResourceRepository, ApiResourceRepository>();
        services.AddScoped<SyncCache>();

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static async Task AddOidcDbContext<T>(this IServiceCollection services, Func<OidcDbContextOptions, Task> options) where T : DbContext, IMasaDbContext
    {
        services.AddOidcDbContext<T>();
        using var scope = services.BuildServiceProvider().CreateScope();
        var oidcDbContextOptions = new OidcDbContextOptions(scope.ServiceProvider);
        await options.Invoke(oidcDbContextOptions);
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
        if(unitOfWork is not null)
        {
            await unitOfWork.CommitAsync();
        }
    }
}
