// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        Action<JsonSerializerOptions>? configure = null)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(configure);

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions,
        Action<JsonSerializerOptions>? configure = null)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(configureOptions, configure);

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        string serializationName)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(serializationName);

    public static IServiceCollection AddMasaIdentity(
        this IServiceCollection services,
        string serializationName,
        Action<IdentityClaimOptions> configureOptions)
        => AddMasaIdentityCore(services).AddMasaIdentityCore(serializationName, configureOptions);

    [Obsolete($"{nameof(AddMasaIdentityModel)} expired, please use {nameof(AddMasaIdentity)}")]
    public static IServiceCollection AddMasaIdentityModel(this IServiceCollection services)
        => services.AddMasaIdentity();

    [Obsolete($"{nameof(AddMasaIdentityModel)} expired, please use {nameof(AddMasaIdentity)}")]
    public static IServiceCollection AddMasaIdentityModel(this IServiceCollection services, Action<IdentityClaimOptions> configureOptions)
        => services.AddMasaIdentity(configureOptions);

    [Obsolete($"{nameof(AddMasaIdentityModel)} expired, please use {nameof(AddMasaIdentity)}")]
    public static IServiceCollection AddMasaIdentityModel(this IServiceCollection services, string serializationName)
        => services.AddMasaIdentity(serializationName);

    [Obsolete($"{nameof(AddMasaIdentityModel)} expired, please use {nameof(AddMasaIdentity)}")]
    public static IServiceCollection AddMasaIdentityModel(
        this IServiceCollection services,
        string serializationName,
        Action<IdentityClaimOptions> configureOptions)
        => services.AddMasaIdentity(serializationName, configureOptions);

    private static IServiceCollection AddMasaIdentityCore(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>();
        return services;
    }
}
