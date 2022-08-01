// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis.Models;

/// <summary>
/// The redis configuration options.
/// </summary>
public class RedisConfigurationOptions : DistributedCacheEntryOptions
{
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
    /// The client name to use for all connections
    /// </summary>
    public string ClientName { get; set; } = default!;

    /// <summary>
    /// Automatically encodes and decodes channels.
    /// </summary>
    public string ChannelPrefix { get; set; } = default!;

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
    public string Password { get; set; }= default!;

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

    public void Initialize(RedisConfigurationOptions options)
    {
        Servers = options.Servers;
        ClientName = options.ClientName;
        ChannelPrefix = options.ChannelPrefix;
        ConnectRetry = options.ConnectRetry;
        ConnectTimeout = options.ConnectTimeout;
        DefaultDatabase = options.DefaultDatabase;
        Password = options.Password;
        Proxy = options.Proxy;
        Ssl = options.Ssl;
        SyncTimeout = options.SyncTimeout;
        AbsoluteExpiration = options.AbsoluteExpiration;
        AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
        SlidingExpiration = options.SlidingExpiration;
    }
}
