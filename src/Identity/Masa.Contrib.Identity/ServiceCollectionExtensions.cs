// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentity(this IServiceCollection services)
        => services.AddMasaIdentity(_ =>
        {
        });

    public static IServiceCollection AddMasaIdentity(this IServiceCollection services, Action<IdentityClaimOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();
        services.TryAddSingleton<ITypeConvertProvider, DefaultTypeConvertProvider>();
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>();

        services.Configure(configureOptions);

        services.TryAddScoped<DefaultUserContext>();
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultUserContext>());
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultUserContext>());
        return services;
    }

    private class IdentityProvider
    {

    }
}
