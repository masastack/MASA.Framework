// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentityModel(
        this IServiceCollection services,
        IdentityType identityType = IdentityType.Basic)
        => services.AddMasaIdentityModel(identityType, _ =>
        {
        });

    public static IServiceCollection AddMasaIdentityModel(
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

        if (identityType.HasFlag(IdentityType.MultiTenant) && identityType.HasFlag(IdentityType.MultiEnvironment))
            return services.AddMasaIdentityByIsolation();

        if (identityType.HasFlag(IdentityType.MultiTenant))
            return services.AddMasaIdentityByMultiTenant();

        if (identityType.HasFlag(IdentityType.MultiEnvironment))
            return services.AddMasaIdentityByMultiEnvironment();

        if ((identityType & IdentityType.Basic) != 0)
            return services.AddMasaIdentityByBasic();

        throw new NotSupportedException(nameof(identityType));
    }

    private static IServiceCollection AddMasaIdentityByBasic(this IServiceCollection services)
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
        services.TryAddScoped<DefaultIsolatedUserContext>();
        services.TryAddScoped<IUserSetter>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolatedUserContext>());
        services.TryAddScoped<IUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolatedUserContext>());
        services.TryAddScoped<IIsolatedUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolatedUserContext>());
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolatedUserContext>());
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider
            => serviceProvider.GetRequiredService<DefaultIsolatedUserContext>());
        return services;
    }

    private sealed class IdentityProvider
    {
    }
}
