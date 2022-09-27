// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        Action<RedisConfigurationOptions> action)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        return services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);
    }

    public static ICachingBuilder AddStackExchangeRedisCache(this IServiceCollection services,
        RedisConfigurationOptions redisConfigurationOptions)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions);

    /// <summary>
    /// Adds a default implementation for the <see cref="T:Masa.BuildingBlocks.Caching.IDistributedCacheClient" /> service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static ICachingBuilder AddStackExchangeRedisCache(this IServiceCollection services)
        => services.AddStackExchangeRedisCache(Microsoft.Extensions.Options.Options.DefaultName);

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
        services.TryAddDistributedCacheCore();

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
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        return services.AddStackExchangeRedisCache(
            name,
            redisConfigurationOptions,
            jsonSerializerOptions);
    }

    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.TryAddDistributedCacheCore();

        services.Configure<RedisConfigurationOptions>(name, options =>
        {
            options.AbsoluteExpiration = redisConfigurationOptions.AbsoluteExpiration;
            options.AbsoluteExpirationRelativeToNow = redisConfigurationOptions.AbsoluteExpirationRelativeToNow;
            options.SlidingExpiration = redisConfigurationOptions.SlidingExpiration;
            options.Servers = redisConfigurationOptions.Servers;
            options.AbortOnConnectFail = redisConfigurationOptions.AbortOnConnectFail;
            options.AllowAdmin = redisConfigurationOptions.AllowAdmin;
            options.ClientName = redisConfigurationOptions.ClientName;
            options.ChannelPrefix = redisConfigurationOptions.ChannelPrefix;
            options.ConnectRetry = redisConfigurationOptions.ConnectRetry;
            options.ConnectTimeout = redisConfigurationOptions.ConnectTimeout;
            options.DefaultDatabase = redisConfigurationOptions.DefaultDatabase;
            options.Password = redisConfigurationOptions.Password;
            options.Proxy = redisConfigurationOptions.Proxy;
            options.Ssl = redisConfigurationOptions.Ssl;
            options.SyncTimeout = redisConfigurationOptions.SyncTimeout;
        });

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
