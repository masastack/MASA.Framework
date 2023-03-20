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
        string redisSectionName = Const.DEFAULT_REDIS_SECTION_NAME,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.Services.AddConfigure<RedisConfigurationOptions>(redisSectionName, distributedCacheBuilder.Name);

        distributedCacheBuilder.AddDistributedCache(serviceProvider =>
        {
            RedisConfigurationOptions? redisConfigurationOptions;
            var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
            if (isolationOptions.Value.Enable)
            {
                redisConfigurationOptions =
                    serviceProvider
                        .GetRequiredService<IIsolationConfigProvider>()
                        .GetModuleConfig<RedisConfigurationOptions>(distributedCacheBuilder.Name, redisSectionName) ?? GetDefaultRedisConfigurationOptions(serviceProvider);
            }
            else
            {
                redisConfigurationOptions = GetDefaultRedisConfigurationOptions(serviceProvider);
            }
            return new RedisCacheClient(
                distributedCacheBuilder.Name,
                redisConfigurationOptions,
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                serviceProvider.GetRequiredService<IRedisMultiplexerProvider>(),
                serviceProvider.GetRequiredService<IFormatCacheKeyProvider>());
        });
        distributedCacheBuilder.Services.UseStackExchangeRedisCacheCore();

        RedisConfigurationOptions GetDefaultRedisConfigurationOptions(IServiceProvider serviceProvider)
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsSnapshot<RedisConfigurationOptions>>();
            return optionsMonitor.Get(distributedCacheBuilder.Name);
        }
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
                distributedCacheBuilder.Name,
                redisConfigurationOptions,
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                serviceProvider.GetRequiredService<IRedisMultiplexerProvider>(),
                serviceProvider.GetRequiredService<IFormatCacheKeyProvider>()
            );
            return distributedCacheClient;
        });
        distributedCacheBuilder.Services.UseStackExchangeRedisCacheCore();
    }

    public static void UseStackExchangeRedisCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        distributedCacheBuilder.AddDistributedCache(serviceProvider =>
        {
            var distributedCacheClient = new RedisCacheClient(
                distributedCacheBuilder.Name,
                redisConfigurationOptions,
                jsonSerializerOptions,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(distributedCacheBuilder.Name),
                serviceProvider.GetRequiredService<IRedisMultiplexerProvider>(),
                serviceProvider.GetRequiredService<IFormatCacheKeyProvider>()
            );
            return distributedCacheClient;
        });
        distributedCacheBuilder.Services.UseStackExchangeRedisCacheCore();
    }

    private static void UseStackExchangeRedisCacheCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IRedisMultiplexerProvider, DefaultRedisMultiplexerProvider>();
    }
}
