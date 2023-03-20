// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class RedisConfigurationOptions : CacheEntryOptions
{
    public string? InstanceId { get; set; }

    /// <summary>
    /// Gets the servers.
    /// </summary>
    public List<RedisServerOptions> Servers { get; set; } = new();

    /// <summary>
    /// Gets or sets whether connect/configuration timeouts should be explicitly notified via a TimeoutException.
    /// </summary>
    public bool AbortOnConnectFail { get; set; }

    /// <summary>
    /// Indicates whether admin operations should be allowed.
    /// </summary>
    public bool AllowAdmin { get; set; }

    /// <summary>
    /// Specifies the time in milliseconds that the system should allow for asynchronous operations (defaults: 5000)
    /// </summary>
    public int AsyncTimeout { get; set; } = 5000;

    /// <summary>
    /// The client name to use for all connections
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Automatically encodes and decodes channels.
    /// </summary>
    public string ChannelPrefix { get; set; } = string.Empty;

    /// <summary>
    /// The number of times to repeat the initial connect cycle if no servers respond promptly.
    /// </summary>
    public int ConnectRetry { get; set; } = 3;

    /// <summary>
    ///  Specifies the time in milliseconds that should be allowed for connection (defaults to 5 seconds unless SyncTimeout is higher)
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.
    /// </summary>
    public int DefaultDatabase { get; set; }

    /// <summary>
    /// The password to use to authenticate with the server.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Type of proxy to use (if any); for example Proxy.Twemproxy.
    /// </summary>
    public Proxy Proxy { get; set; } = Proxy.None;

    /// <summary>
    /// Indicates whether the connection should be encrypted.
    /// </summary>
    public bool Ssl { get; set; }

    /// <summary>
    /// Specifies the time in milliseconds that the system should allow for synchronous operations (defaults to 5 seconds)
    /// </summary>
    public int SyncTimeout { get; set; } = 1000;



    public CacheOptions GlobalCacheOptions { get; set; } = new()
    {
        CacheKeyType = CacheKeyType.TypeName
    };

    public static implicit operator ConfigurationOptions(RedisConfigurationOptions options)
    {
        var configurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = options.AbortOnConnectFail,
            AllowAdmin = options.AllowAdmin,
            AsyncTimeout = options.AsyncTimeout,
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
        return configurationOptions;
    }
}
