// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Caching.Redis.Models;

namespace Masa.Contrib.Oidc.Cache.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCacheStorage(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddOidcCache(options);
        services.AddSingleton<IClientStore, ClientStore>();
        services.AddSingleton<IResourceStore, ResourceStore>();
        services.AddSingleton<IPersistedGrantStore, PersistedGrantStore>();
        services.AddSingleton<IDeviceFlowStore, DeviceFlowStore>();

        return services;
    }
}
