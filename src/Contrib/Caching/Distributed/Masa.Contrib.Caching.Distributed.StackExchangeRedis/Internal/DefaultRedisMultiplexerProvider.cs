// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal class DefaultRedisMultiplexerProvider : IRedisMultiplexerProvider
{
    private readonly MemoryCache<string, IConnectionMultiplexer> _data = new();

    public IConnectionMultiplexer GetConnectionMultiplexer(string name, RedisConfigurationOptions redisConfigurationOptions)
        => _data.GetOrAdd(ConvertToKey(name, redisConfigurationOptions), _ => ConnectionMultiplexer.Connect(redisConfigurationOptions));

    public void TryRemove(string name, RedisConfigurationOptions redisConfigurationOptions)
    {
        var key = ConvertToKey(name, redisConfigurationOptions);
        if (_data.TryGet(key, out IConnectionMultiplexer? connectionMultiplexer))
        {
            connectionMultiplexer.Dispose();
            _data.Remove(key);
        }
    }

    private static string ConvertToKey(string name, RedisConfigurationOptions redisConfigurationOptions)
        => $"{name}{JsonSerializer.Serialize(redisConfigurationOptions)}";
}
