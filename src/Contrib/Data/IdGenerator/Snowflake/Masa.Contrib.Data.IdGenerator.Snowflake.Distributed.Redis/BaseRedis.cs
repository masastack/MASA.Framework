// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class BaseRedis
{
    protected IDistributedCacheClient DistributedCacheClient { get; }
    protected ConnectionMultiplexer Connection { get; }
    internal IDatabase Database { get; }

    public BaseRedis(RedisConfigurationOptions redisOptions)
    {
        DistributedCacheClient = new RedisCacheClient(redisOptions);
        var options = (ConfigurationOptions)redisOptions;
        Connection = ConnectionMultiplexer.Connect(options);
        Database = Connection.GetDatabase(options.DefaultDatabase ?? 0);
    }
}
