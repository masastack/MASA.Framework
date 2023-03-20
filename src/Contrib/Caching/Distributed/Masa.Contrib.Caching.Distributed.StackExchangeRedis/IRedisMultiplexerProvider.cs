// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Provide Redis reuse Connection
/// </summary>
public interface IRedisMultiplexerProvider
{
    IConnectionMultiplexer GetConnectionMultiplexer(string name, RedisConfigurationOptions redisConfigurationOptions);

    void TryRemove(string name, RedisConfigurationOptions redisConfigurationOptions);
}
