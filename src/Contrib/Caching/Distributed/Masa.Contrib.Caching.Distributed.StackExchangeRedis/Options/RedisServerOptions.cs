// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class RedisServerOptions
{
    /// <summary>
    /// Gets the host.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets the port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    public RedisServerOptions()
    {
        Host = RedisConstant.DEFAULT_REDIS_HOST;
        Port = RedisConstant.DEFAULT_REDIS_PORT;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    /// <param name="host">The host.</param>
    public RedisServerOptions(string host)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(host);

        var lastIndex = host.LastIndexOf(':');
        if (lastIndex > 0 && host.Length > lastIndex + 1 && int.TryParse(host.AsSpan(lastIndex + 1), out var port))
        {
            Host = host.Substring(0, lastIndex);
            Port = port;
        }

        if (string.IsNullOrEmpty(Host))
        {
            Host = host;
            Port = RedisConstant.DEFAULT_REDIS_PORT;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    public RedisServerOptions(string host, int port)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(host);

        if (port <= 0)
            throw new ArgumentOutOfRangeException(nameof(port), $"{nameof(port)} must be greater than 0");

        Host = host;
        Port = port;
    }
}
