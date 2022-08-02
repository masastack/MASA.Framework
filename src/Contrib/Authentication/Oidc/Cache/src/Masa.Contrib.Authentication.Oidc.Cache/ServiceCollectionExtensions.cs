// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache;

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
