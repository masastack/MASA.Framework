// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services)
        => AddMasaIdentityCore(services).AddMasaIdentityCore();

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(configureOptions);

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        string serializationName)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(serializationName);

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        string serializationName,
        Action<IdentityClaimOptions> configureOptions)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(serializationName, configureOptions);

    private static IServiceCollection AddMasaIdentityCore(IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.TryAddScoped<ICurrentPrincipalAccessor, BlazorCurrentPrincipalAccessor>();
        return services;
    }
}
