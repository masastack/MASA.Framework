// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        IdentityType identityType = IdentityType.Simple)
        => services.AddMasaIdentity(identityType, _ =>
        {
        });

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        IdentityType identityType,
        Action<IdentityClaimOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();
        services.TryAddSingleton<ITypeConvertProvider, DefaultTypeConvertProvider>();
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>();

        services.Configure(configureOptions);

        switch (identityType)
        {
            case IdentityType.Simple:
                return services.AddMasaIdentityBySimple();
            case IdentityType.MultiTenant:
                return services.AddMasaIdentityByMultiTenant();
            case IdentityType.MultiEnvironment:
                return services.AddMasaIdentityByMultiEnvironment();
            case IdentityType.Isolation:
                return services.AddMasaIdentityByIsolation();
            default:
                throw new NotSupportedException(nameof(identityType));
        }
    }

    private static IServiceCollection AddMasaIdentityBySimple(this IServiceCollection services)
    {
        services.TryAddScoped<DefaultUserContext>();
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultUserContext>());
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultUserContext>());
        return services;
    }

    private static IServiceCollection AddMasaIdentityByMultiTenant(this IServiceCollection services)
    {
        services.TryAddScoped<DefaultMultiTenantUserContext>();
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiTenantUserContext>());
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiTenantUserContext>());
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiTenantUserContext>());
        return services;
    }

    private static IServiceCollection AddMasaIdentityByMultiEnvironment(this IServiceCollection services)
    {
        services.TryAddScoped<DefaultMultiEnvironmentUserContext>();
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiEnvironmentUserContext>());
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiEnvironmentUserContext>());
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultMultiEnvironmentUserContext>());
        return services;
    }

    private static IServiceCollection AddMasaIdentityByIsolation(this IServiceCollection services)
    {
        services.TryAddScoped<DefaultIsolationUserContext>();
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolationUserContext>());
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolationUserContext>());
        services.TryAddScoped<IIsolationUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolationUserContext>());
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolationUserContext>());
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolationUserContext>());
        return services;
    }

    private class IdentityProvider
    {
    }
}
