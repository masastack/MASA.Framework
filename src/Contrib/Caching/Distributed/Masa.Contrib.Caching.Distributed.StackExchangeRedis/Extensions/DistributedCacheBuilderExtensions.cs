// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public static class DistributedCacheBuilderExtensions
{
    /// <summary>
    /// Add distributed Redis cache
    /// </summary>
    /// <param name="distributedCacheBuilder"></param>
    /// <param name="redisSectionName">redis node name, not required, default: RedisConfig(Use local configuration)</param>
    /// <param name="jsonSerializerOptions"></param>
    /// <returns></returns>
    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        string redisSectionName = RedisConstant.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.Services.AddConfigure<RedisConfigurationOptions>(redisSectionName, distributedCacheBuilder.Name);

        distributedCacheBuilder.AddDistributedCache(serviceProvider =>
        {
            var redisConfigurationOptions = ComponentConfigUtils.GetComponentConfigByExecute(
                serviceProvider,
                distributedCacheBuilder.Name,
                redisSectionName,
                () =>
                {
                    var optionsMonitor = serviceProvider.GetRequiredService<IOptionsSnapshot<RedisConfigurationOptions>>();
                    return optionsMonitor.Get(distributedCacheBuilder.Name);
                });

            return new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name));
        });
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.AddDistributedCache(serviceProvider =>
        {
            var redisConfigurationOptions = new RedisConfigurationOptions();
            action.Invoke(redisConfigurationOptions);
            var distributedCacheClient = new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name)
            );
            return distributedCacheClient;
        });
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.AddDistributedCache(serviceProvider =>
        {
            var distributedCacheClient = new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name)
            );
            return distributedCacheClient;
        });
    }
}
