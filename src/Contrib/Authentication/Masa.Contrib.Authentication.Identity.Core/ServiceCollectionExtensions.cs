// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        Action<JsonSerializerOptions>? configure = null)
        => services.AddMasaIdentityCore(_ =>
        {

        }, configure);

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions,
        Action<JsonSerializerOptions>? configure = null)
    {
        services.AddJson(Constants.DEFAULT_IDENTITY_NAME, configure);
        return services.AddMasaIdentityCore(Constants.DEFAULT_IDENTITY_NAME, configureOptions);
    }

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        string serializationName)
        => services.AddMasaIdentityCore(serializationName, _ =>
        {
        });

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        string serializationName,
        Action<IdentityClaimOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();

        services.AddTypeConvert(serializationName);

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore(serializationName);
    }

    private static IServiceCollection AddMasaIdentityModelCore(this IServiceCollection services, string serializationName)
    {
        services.TryAddScoped(serviceProvider => new DefaultUserContext(
            serviceProvider.GetRequiredService<ITypeConvertFactory>().Create(serializationName),
            serviceProvider.GetRequiredService<ICurrentPrincipalAccessor>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>()));

        services.TryAddScoped<IUserSetter>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IUserContext>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IMultiTenantUserContext, DefaultMultiTenantUserContext>();
        services.TryAddScoped<IMultiEnvironmentUserContext, DefaultMultiEnvironmentUserContext>();
        services.TryAddScoped<IIsolatedUserContext, DefaultIsolatedUserContext>();
        return services;
    }

    private sealed class IdentityProvider
    {
    }
}
