// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class BaseRedis
{
    protected IDistributedCacheClient DistributedCacheClient { get; }
    internal ConnectionMultiplexer Connection { get; }
    internal IDatabase Database { get; }

    public BaseRedis(IDistributedCacheClient distributedCacheClient, IOptions<RedisConfigurationOptions> redisOptions)
    {
        DistributedCacheClient = distributedCacheClient;
        var options = GetConfigurationOptions(redisOptions.Value);
        Connection = ConnectionMultiplexer.Connect(options);
        Database = Connection.GetDatabase(options.DefaultDatabase ?? 0);
    }

    private ConfigurationOptions GetConfigurationOptions(RedisConfigurationOptions redisOptions)
    {
        var configurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = redisOptions.AbortOnConnectFail,
            AllowAdmin = redisOptions.AllowAdmin,
            ChannelPrefix = redisOptions.ChannelPrefix,
            ClientName = redisOptions.ClientName,
            ConnectRetry = redisOptions.ConnectRetry,
            ConnectTimeout = redisOptions.ConnectTimeout,
            DefaultDatabase = redisOptions.DefaultDatabase,
            Password = redisOptions.Password,
            Proxy = redisOptions.Proxy,
            Ssl = redisOptions.Ssl,
            SyncTimeout = redisOptions.SyncTimeout
        };

        foreach (var server in redisOptions.Servers)
        {
            configurationOptions.EndPoints.Add(server.Host, server.Port);
        }
        return configurationOptions;
    }
}
