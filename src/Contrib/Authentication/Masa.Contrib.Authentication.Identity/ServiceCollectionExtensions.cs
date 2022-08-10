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
        => services.AddMasaIdentityModel(DataType.Json.ToString(), configureOptions);

    public static IServiceCollection AddMasaIdentityModel(
        this IServiceCollection services,
        string name,
        Action<IdentityClaimOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();

        services.AddJson(DataType.Json.ToString());
        services.AddTypeConvert(DataType.Json.ToString());
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>();

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore(name);
    }

    private static IServiceCollection AddMasaIdentityModelCore(this IServiceCollection services, string name)
    {
        services.TryAddScoped(serviceProvider => new DefaultUserContext(
            serviceProvider.GetRequiredService<ITypeConvertFactory>().Create(name),
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
