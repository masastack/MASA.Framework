// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackExchangeRedisCache(this IServiceCollection services,
        RedisConfigurationOptions redisConfigurationOptions)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);

    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddStackExchangeRedisCacheCore();

        services.TryAddConfigure<RedisConfigurationOptions>(redisSectionName, name);

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClient = new DistributedCacheClient(
                    serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>(),
                    name,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return services;
    }

    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddStackExchangeRedisCacheCore();

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, _ =>
            {
                var distributedCacheClient = new DistributedCacheClient(
                    redisConfigurationOptions,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return services;
    }

    private static void AddStackExchangeRedisCacheCore(this IServiceCollection services)
    {
        if (services.Any(service => service.ImplementationType == typeof(RedisProvider)))
        {
            return;
        }
        services.AddSingleton<RedisProvider>();
        services.AddCachingCore();
    }

    private sealed class RedisProvider
    {

    }
}
