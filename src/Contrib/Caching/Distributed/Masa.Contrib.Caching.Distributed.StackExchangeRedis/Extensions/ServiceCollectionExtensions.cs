// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ICachingBuilder AddStackExchangeRedisCache(this IServiceCollection services,
        RedisConfigurationOptions redisConfigurationOptions)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);

    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="redisSectionName">redis node name, not required, default: RedisConfig(Use local configuration)</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddDistributedCacheCore();

        services.AddConfigure<RedisConfigurationOptions>(redisSectionName, name);

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

        return new CachingBuilder(services, name);
    }

    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddDistributedCacheCore();

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

        return new CachingBuilder(services, name);
    }
}
