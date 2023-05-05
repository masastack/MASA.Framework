// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisConfigurationOptionsExtensions
{
    /// <summary>
    /// Get the available redis configuration
    /// </summary>
    /// <param name="redisConfigurationOptions"></param>
    /// <returns></returns>
    public static RedisConfigurationOptions GetAvailableRedisOptions(this RedisConfigurationOptions redisConfigurationOptions)
    {
        if (redisConfigurationOptions.Servers.Any())
            return redisConfigurationOptions;

        return new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            }
        };
    }
}
