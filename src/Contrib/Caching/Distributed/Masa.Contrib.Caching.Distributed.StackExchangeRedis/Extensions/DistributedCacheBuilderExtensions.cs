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
        distributedCacheBuilder.Services.AddScoped(serviceProvider => ConnectionMultiplexer.Connect(GetRedisConfig(serviceProvider, distributedCacheBuilder, redisSectionName).GetAvailableRedisOptions()));
        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider => new RedisCacheClient(
                serviceProvider.GetRequiredService<ConnectionMultiplexer>(),
                GetRedisConfig(serviceProvider, distributedCacheBuilder, redisSectionName),
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name))
        );
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        Action<RedisConfigurationOptions> action,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        action.Invoke(redisConfigurationOptions);
        distributedCacheBuilder.Services.AddScoped(serviceProvider => ConnectionMultiplexer.Connect(redisConfigurationOptions.GetAvailableRedisOptions()));
        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider => new RedisCacheClient(
                serviceProvider.GetRequiredService<ConnectionMultiplexer>(),
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name)
            ));
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.Services.AddScoped(serviceProvider => ConnectionMultiplexer.Connect(redisConfigurationOptions.GetAvailableRedisOptions()));
        distributedCacheBuilder.UseCustomDistributedCache(serviceProvider =>
        {
            var distributedCacheClient = new RedisCacheClient(
                serviceProvider.GetRequiredService<ConnectionMultiplexer>(),
                redisConfigurationOptions,
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name)
            );
            return distributedCacheClient;
        });
    }

    private static RedisConfigurationOptions GetRedisConfig(IServiceProvider serviceProvider, DistributedCacheBuilder distributedCacheBuilder, string redisSectionName)
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
        return redisConfigurationOptions;
    }
}
