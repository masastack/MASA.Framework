// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackExchangeRedisCache(this IServiceCollection services,
        DistributedRedisCacheOptions? distributedRedisCacheOptions)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            distributedRedisCacheOptions);

    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        DistributedRedisCacheOptions? distributedRedisCacheOptions)
    {
        services.AddStackExchangeRedisCacheCore(out bool isSkip);
        if (isSkip) return services;

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            // todo: execute only once
            // if (options.Options.Any(opt => opt.Name == name))
            //     return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClient = new DistributedCacheClient(
                    serviceProvider.GetRequiredService<IOptionsMonitor<DistributedRedisCacheOptions>>(),
                    distributedRedisCacheOptions
                );

                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return services;
    }

    private static void AddStackExchangeRedisCacheCore(this IServiceCollection services, out bool isSkip)
    {
        if (services.Any(service => service.ImplementationType == typeof(RedisProvider)))
        {
            isSkip = true;
        }

        isSkip = false;
        services.AddSingleton<RedisProvider>();

        services.AddDefaultConfigurationByStackExchangeRedisCache();
        services.AddCachingCore();
    }

    public static IServiceCollection AddDefaultConfigurationByStackExchangeRedisCache(this IServiceCollection services)
    {
        services.Configure<DistributedRedisCacheOptions>(options =>
        {
            options.Options ??= new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            };

            options.JsonSerializerOptions ??= new JsonSerializerOptions().EnableDynamicTypes();

            options.SubscribeConfigurationOptions ??= new SubscribeConfigurationOptions()
            {
                SubscribeKeyTypes = SubscribeKeyTypes.ValueTypeFullNameAndKey
            };

            options.CacheEntryOptions ??= new CacheEntryOptions();
        });
        return services;
    }

    private sealed class RedisProvider
    {

    }
}
