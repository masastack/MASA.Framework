// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Caching.MultilevelCache;

namespace Masa.Contrib.Authentication.Oidc.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCache(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("RedisConfig").Get<RedisConfigurationOptions>();
        services.AddOidcCache(options);

        return services;
    }

    public static IServiceCollection AddOidcCache(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddStackExchangeRedisCache(Constants.DEFAULT_CLIENT_NAME, options).AddMultilevelCache(new MultilevelCacheOptions()
        {
            SubscribeKeyType = SubscribeKeyType.SpecificPrefix,
            SubscribeKeyPrefix = Constants.DEFAULT_SUBSCRIBE_KEY_PREFIX
        });
        services.AddSingleton<MemoryCacheProvider>();
        services.AddSingleton<IClientCache, ClientCache>();
        services.AddSingleton<IApiScopeCache, ApiScopeCache>();
        services.AddSingleton<IApiResourceCache, ApiResourceCache>();
        services.AddSingleton<IIdentityResourceCache, IdentityResourceCache>();

        return services;
    }
}
