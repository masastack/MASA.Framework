// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Oidc.Cache.Caches;
using Masa.Utils.Caching.Redis.Models;

namespace Masa.Contrib.Oidc.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCache(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddMasaRedisCache(options).AddMasaMemoryCache();
        services.AddSingleton<IClientCache, ClientCache>();
        services.AddSingleton<IApiScopeCache, ApiScopeCache>();
        services.AddSingleton<IApiResourceCache, ApiResourceCache>();
        services.AddSingleton<IIdentityResourceCache, IdentityResourceCache>();

        return services;
    }
}
