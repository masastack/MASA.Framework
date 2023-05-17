// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal static class CacheUtils
{
    public static IManualDistributedCacheClient CreateDistributedCacheClient(
        DccConfigurationOptions dccConfigurationOptions,
        IFormatCacheKeyProvider? formatCacheKeyProvider)
        => new RedisCacheClient(dccConfigurationOptions.RedisOptions, formatCacheKeyProvider);

    // 需考虑同一个实例应该使用一个 CacheClient
    public static IManualDistributedCacheClient CreateDistributedCacheClient(
        DccConfigurationOptions dccConfigurationOptions,
        IServiceProvider serviceProvider)
    {
        var formatCacheKeyProvider = serviceProvider.GetService<IFormatCacheKeyProvider>();
        return new RedisCacheClient(dccConfigurationOptions.RedisOptions, formatCacheKeyProvider);
    }

    public static IManualMultilevelCacheClient CreateMultilevelCacheClient(
        DccConfigurationOptions dccConfigurationOptions,
        IFormatCacheKeyProvider? formatCacheKeyProvider)
    {
        var multilevelCacheGlobalOptions = new MultilevelCacheGlobalOptions
        {
            SubscribeKeyType = SubscribeKeyType.SpecificPrefix,
            SubscribeKeyPrefix = dccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX
        };

        var distributedCacheClient = CreateDistributedCacheClient(dccConfigurationOptions, formatCacheKeyProvider);
        var multilevelCacheClient = new MultilevelCacheClient(
            new MemoryCache(multilevelCacheGlobalOptions),
            distributedCacheClient,
            new MultilevelCacheOptions
            {
                CacheKeyType = multilevelCacheGlobalOptions.GlobalCacheOptions.CacheKeyType,
                MemoryCacheEntryOptions = multilevelCacheGlobalOptions.CacheEntryOptions
            },
            multilevelCacheGlobalOptions.SubscribeKeyType,
            multilevelCacheGlobalOptions.SubscribeKeyPrefix,
            null,
            formatCacheKeyProvider,
            multilevelCacheGlobalOptions.InstanceId
        );
        return multilevelCacheClient;
    }
}
