// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis.Models;

/// <summary>
/// The redis configuration.
/// </summary>
public class RedisServerOptions
{
    private const string DEFAULT_REDIS_HOST = "localhost";
    private const int DEFAULT_REDIS_PORT = 6379;

    /// <summary>
    /// Gets the host.
    /// </summary>
    public string Host { get; set; } = default!;

    /// <summary>
    /// Gets the port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    public RedisServerOptions()
    {
        Host = DEFAULT_REDIS_HOST;
        Port = DEFAULT_REDIS_PORT;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    /// <param name="host">The host.</param>
    public RedisServerOptions(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentNullException(nameof(host));
        }

        var lastIndex = host.LastIndexOf(':');
        if (lastIndex > 0 && host.Length > lastIndex + 1)
        {
            if (int.TryParse(host.Substring(lastIndex + 1), out var port))
            {
                Host = host.Substring(0, lastIndex);
                Port = port;
            }
        }

        if (string.IsNullOrEmpty(Host))
        {
            Host = host;
            Port = DEFAULT_REDIS_PORT;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisServerOptions"/> class.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="port">The port.</param>
    public RedisServerOptions(string host, int port)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentNullException(nameof(host));

        Host = host;
        Port = port;
    }
}
