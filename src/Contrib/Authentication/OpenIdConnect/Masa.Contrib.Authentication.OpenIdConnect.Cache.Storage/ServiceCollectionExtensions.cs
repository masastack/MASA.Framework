// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Caching.Distributed.StackExchangeRedis;

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Storage;

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

        return services;
    }
}
