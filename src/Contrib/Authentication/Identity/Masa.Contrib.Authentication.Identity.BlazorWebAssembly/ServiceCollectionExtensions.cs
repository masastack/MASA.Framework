// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

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

    private static IServiceCollection AddMasaIdentityCore(IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.TryAddSingleton<IClientScopeServiceProviderAccessor, ComponentsClientScopeServiceProviderAccessor>();
        services.TryAddScoped<ICurrentPrincipalAccessor, BlazorCurrentPrincipalAccessor>();
        return services;
    }

    public static async Task InitializeApplicationAsync(
        [NotNull] this IServiceProvider serviceProvider)
    {
        ((ComponentsClientScopeServiceProviderAccessor)serviceProvider
            .GetRequiredService<IClientScopeServiceProviderAccessor>()).ServiceProvider = serviceProvider;
        await serviceProvider.GetRequiredService<IClientScopeServiceProviderAccessor>().ServiceProvider.GetRequiredService<MasaComponentsClaimsCache>().InitializeAsync();
    }
}
