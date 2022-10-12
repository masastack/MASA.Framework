// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="distributedOptions"></param>
    /// <param name="redisSectionName">redis node name, not required, default: RedisConfig(Use local configuration)</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static void UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.AddConfigure<RedisConfigurationOptions>(redisSectionName, distributedOptions.Name);

        distributedOptions.Services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == distributedOptions.Name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(distributedOptions.Name, serviceProvider =>
            {
                var distributedCacheClient = new RedisCacheClient(
                    serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>(),
                    distributedOptions.Name,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        distributedOptions.UseStackExchangeRedisCache(redisConfigurationOptions, jsonSerializerOptions);
    }

    public static void UseStackExchangeRedisCache(this DistributedCacheOptions distributedOptions,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.Configure<RedisConfigurationOptions>(distributedOptions.Name, options =>
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

        distributedOptions.Services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == distributedOptions.Name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(distributedOptions.Name, _ =>
            {
                var distributedCacheClient = new RedisCacheClient(
                    redisConfigurationOptions,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });
    }

    #region Obsolete

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
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

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
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
    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
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
    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddDistributedCacheCore(services);
        new DistributedCacheOptions(services, name).UseStackExchangeRedisCache(redisSectionName, jsonSerializerOptions);
        return new CachingBuilder(services, name);
    }

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
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

    [Obsolete(
        "AddStackExchangeRedisCache has expired, please use services.AddDistributedCache(options => options.UseStackExchangeRedisCache()) or services.AddMultilevelCache(options => options.UseStackExchangeRedisCache()) instead")]
    public static ICachingBuilder AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddDistributedCacheCore(services);

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
                var distributedCacheClient = new RedisCacheClient(
                    redisConfigurationOptions,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return new CachingBuilder(services, name);
    }

    #endregion

}
