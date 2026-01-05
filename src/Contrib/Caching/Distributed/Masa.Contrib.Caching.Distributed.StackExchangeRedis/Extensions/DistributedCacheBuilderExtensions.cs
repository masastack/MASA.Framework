// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public static class DistributedCacheBuilderExtensions
{
    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        string redisSectionName = RedisConstant.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null,
        Action<IConnectionMultiplexer>? connectConfig = null)
    {
        distributedCacheBuilder.Services.AddConfigure<RedisConfigurationOptions>(redisSectionName, distributedCacheBuilder.Name);

        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider =>
        {
            var redisConfigurationOptions = ComponentConfigUtils.GetComponentConfigByExecute(
                serviceProvider,
                distributedCacheBuilder.Name,
                redisSectionName,
                () =>
                {
                    if (serviceProvider.EnableIsolation())
                        return serviceProvider.GetRequiredService<IOptionsSnapshot<RedisConfigurationOptions>>()
                            .Get(distributedCacheBuilder.Name);

                    var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();
                    return optionsMonitor.Get(distributedCacheBuilder.Name);
                });

            return new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                connectConfig);
        });
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null,
        Action<IConnectionMultiplexer>? connectConfig = null)
    {
        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider =>
        {
            var redisConfigurationOptions = new RedisConfigurationOptions();
            action.Invoke(redisConfigurationOptions);
            var distributedCacheClient = new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                connectConfig
            );
            return distributedCacheClient;
        });
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null,
        Action<IConnectionMultiplexer>? connectConfig = null)
    {
        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider =>
        {
            var distributedCacheClient = new RedisCacheClient(
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                connectConfig
            );
            return distributedCacheClient;
        });
    }
}
