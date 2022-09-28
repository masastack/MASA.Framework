// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCacheStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOidcCache(configuration);
        services.AddOidcCacheStorage();

        return services;
    }

    public static IServiceCollection AddOidcCacheStorage(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddOidcCache(options);
        services.AddOidcCacheStorage();

        return services;
    }

    static IServiceCollection AddOidcCacheStorage(this IServiceCollection services)
    {
        services.AddSingleton<IClientStore, ClientStore>();
        services.AddSingleton<IResourceStore, ResourceStore>();
        services.AddSingleton<IPersistedGrantStore, PersistedGrantStore>();
        services.AddSingleton<IDeviceFlowStore, DeviceFlowStore>();

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
