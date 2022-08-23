// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions)
    {
        if (services.Any(service => service.ImplementationType == typeof(RedisProvider)))
            return services;

        services.AddSingleton<RedisProvider>();



        services.AddCachingCore();
        return services;
    }

    private sealed class RedisProvider
    {

    }
}
