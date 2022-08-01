// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis;

public class RedisCacheClientFactory : DistributedCacheClientFactory<RedisConfigurationOptions>
{
    public RedisCacheClientFactory(IOptionsMonitor<RedisConfigurationOptions> optionsMonitor) : base(optionsMonitor)
    {

    }

    protected override IDistributedCacheClient CreateClientHandler(string name)
    {
        var options = GetOptions(name);

        if (options == null)
        {
            throw new ArgumentException("No matching client found!");
        }

        var configurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = options.AbortOnConnectFail,
            AllowAdmin = options.AllowAdmin,
            ChannelPrefix = options.ChannelPrefix,
            ClientName = options.ClientName,
            ConnectRetry = options.ConnectRetry,
            ConnectTimeout = options.ConnectTimeout,
            DefaultDatabase = options.DefaultDatabase,
            Password = options.Password,
            Proxy = options.Proxy,
            Ssl = options.Ssl,
            SyncTimeout = options.SyncTimeout
        };

        foreach (var server in options.Servers)
        {
            configurationOptions.EndPoints.Add(server.Host, server.Port);
        }

        return new RedisCacheClient(configurationOptions);
    }
}
