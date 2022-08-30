// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

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

        services.TryAddConfigure<RedisConfigurationOptions>(redisSectionName);

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, serviceProvider =>
            {
                IOptionsMonitor<RedisConfigurationOptions> redisConfigurationOptionsMonitor =
                    serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();

                redisConfigurationOptionsMonitor.OnChange((option, optionName) =>
                {
                    if (optionName == name)
                    {
                        var func = options.Options.First(opt => opt.Name == name);

                        var client = func.Func.Invoke(serviceProvider);
                        if (client is DistributedCacheClient redisClient)
                            redisClient.RefreshRedisConfigurationOptions(option);
                    }
                });

                var distributedCacheClient = new DistributedCacheClient(
                    redisConfigurationOptionsMonitor.Get(name),
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
