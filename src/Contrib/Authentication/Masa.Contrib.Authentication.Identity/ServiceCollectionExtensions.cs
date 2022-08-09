// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentityModel(
        this IServiceCollection services)
        => services.AddMasaIdentityModel(_ =>
        {

        });

    public static IServiceCollection AddMasaIdentityModel(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();

        services.AddMasaData();
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>();

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore();
    }

    private static IServiceCollection AddMasaIdentityModelCore(this IServiceCollection services)
    {
        services.TryAddScoped(serviceProvider => new DefaultUserContext(serviceProvider.GetRequiredService<ITypeConvertProvider>(),
            serviceProvider.GetRequiredService<ICurrentPrincipalAccessor>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>(),
            serviceProvider.GetRequiredService<IJsonDeserializer>()));

        services.TryAddScoped<IUserSetter>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IUserContext>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IMultiTenantUserContext, DefaultMultiTenantUserContext>();
        services.TryAddScoped<IMultiEnvironmentUserContext, DefaultMultiEnvironmentUserContext>();
        services.TryAddScoped<IIsolatedUserContext, DefaultIsolatedUserContext>();
        return services;
    }

    private class IdentityProvider
    {
    }
}
