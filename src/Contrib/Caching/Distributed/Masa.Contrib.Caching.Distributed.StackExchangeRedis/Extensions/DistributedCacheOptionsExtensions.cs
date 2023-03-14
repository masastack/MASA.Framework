// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching;

public static class DistributedCacheOptionsExtensions
{
    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="distributedOptions"></param>
    /// <param name="redisSectionName">redis node name, not required, default: RedisConfig(Use local configuration)</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static DistributedCacheOptions UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.AddConfigure<RedisConfigurationOptions>(redisSectionName, distributedOptions.Name);
        return distributedOptions.UseStackExchangeRedisCache(jsonSerializerOptions);
    }

    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="distributedOptions"></param>
    /// <param name="configuration"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static DistributedCacheOptions UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        IConfiguration configuration,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.Configure<RedisConfigurationOptions>(distributedOptions.Name, configuration);
        return distributedOptions.UseStackExchangeRedisCache(jsonSerializerOptions);
    }

    public static DistributedCacheOptions UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        distributedOptions.UseStackExchangeRedisCache(redisConfigurationOptions, jsonSerializerOptions);
        return distributedOptions;
    }

    public static DistributedCacheOptions UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.UseStackExchangeRedisCache(
            distributedOptions.Name,
            redisConfigurationOptions,
            jsonSerializerOptions);
        return distributedOptions;
    }

    #region internal methods

    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="distributedOptions"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    private static DistributedCacheOptions UseStackExchangeRedisCache(
        this DistributedCacheOptions distributedOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedOptions.Services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == distributedOptions.Name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IManualDistributedCacheClient>(distributedOptions.Name, serviceProvider =>
            {
                var distributedCacheClient = new RedisCacheClient(
                    serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>(),
                    distributedOptions.Name,
                    jsonSerializerOptions,
                    serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedOptions.Name)
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });
        return distributedOptions;
    }

    internal static void UseStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
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

            var cacheRelationOptions = new CacheRelationOptions<IManualDistributedCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClient = new RedisCacheClient(
                    redisConfigurationOptions,
                    jsonSerializerOptions,
                    serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(name)
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });
    }

    #endregion

}
