// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Development.DaprStarters;

/// <summary>
/// dapr startup configuration information
/// When the specified attribute is configured as null, the default value of the parameter is subject to the default value of dapr of the current version
/// </summary>
public class DaprOptions
{
    /// <summary>
    /// The id for your application, used for service discovery
    /// </summary>
    public string? AppId { get; set; }

    private string _appIdDelimiter = Constant.DEFAULT_APPID_DELIMITER;

    /// <summary>
    /// Separator used to splice AppId and AppIdSuffix
    /// default:- , AppIdDelimiter not support .
    /// Not Supported .
    /// </summary>
    public string AppIdDelimiter
    {
        get => _appIdDelimiter;
        set
        {
            if (value == ".")
            {
                throw new NotSupportedException("AppIdDelimiter is not supported as .");
            }

            _appIdDelimiter = value;
        }
    }

    private string? _appIdSuffix;

    /// <summary>
    /// Appid suffix
    /// Defaults to the current MAC address
    /// Not Supported .
    /// </summary>
    public string? AppIdSuffix
    {
        get => _appIdSuffix;
        set
        {
            if (value == ".")
            {
                throw new NotSupportedException("AppIdSuffix is not supported as .");
            }

            _appIdSuffix = value;
        }
    }

    /// <summary>
    /// Whether to disable AppIdSuffix
    /// default: false
    /// </summary>
    public bool DisableAppIdSuffix { get; set; } = false;

    private int? _maxConcurrency;

    /// <summary>
    /// The concurrency level of the application, otherwise is unlimited
    /// Must be greater than 0
    /// </summary>
    public int? MaxConcurrency
    {
        get => _maxConcurrency;
        set
        {
            if (value is <= 0)
            {
                throw new NotSupportedException($"{nameof(MaxConcurrency)} must be greater than 0 .");
            }

            _maxConcurrency = value;
        }
    }

    private ushort? _appPort;

    /// <summary>
    /// The port your application is listening on
    /// Required. Must be greater than 0
    /// </summary>
    public ushort? AppPort
    {
        get => _appPort;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(AppPort)} must be greater than 0 .");

            _appPort = value;
        }
    }

    /// <summary>
    /// The protocol (gRPC or HTTP) Dapr uses to talk to the application. Valid values are: http or grpc
    /// </summary>
    public Protocol? AppProtocol { get; set; }

    /// <summary>
    /// Enable https when Dapr invokes the application
    /// default: null (don't use https)
    /// </summary>
    public bool? EnableSsl { get; set; }

    /// <summary>
    /// Dapr configuration file
    /// default:
    /// Linux & Mac: $HOME/.dapr/config.yaml
    /// Windows: %USERPROFILE%\.dapr\config.yaml
    /// </summary>
    public string? Config { get; set; }

    /// <summary>
    /// The path for components directory
    /// default:
    /// Linux & Mac: $HOME/.dapr/components
    /// Windows: %USERPROFILE%\.dapr\components
    /// </summary>
    public string? ComponentPath { get; set; }

    private ushort? _daprGrpcPort;

    /// <summary>
    /// The gRPC port for Dapr to listen on
    /// Must be greater than 0
    /// </summary>
    public ushort? DaprGrpcPort
    {
        get => _daprGrpcPort;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(DaprGrpcPort)} must be greater than 0 .");

            _daprGrpcPort = value;
        }
    }

    private ushort? _daprHttpPort;

    /// <summary>
    /// The HTTP port for Dapr to listen on
    /// Must be greater than 0
    /// </summary>
    public ushort? DaprHttpPort
    {
        get => _daprHttpPort;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(DaprHttpPort)} must be greater than 0 .");

            _daprHttpPort = value;
        }
    }

    /// <summary>
    /// Enable pprof profiling via an HTTP endpoint
    /// </summary>
    public bool? EnableProfiling { get; set; }

    /// <summary>
    /// The image to build the code in. Input is: repository/image
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// The log verbosity. Valid values are: debug, info, warn, error, fatal, or panic
    /// default: info
    /// </summary>
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// default: localhost
    /// </summary>
    public string? PlacementHostAddress { get; set; }

    /// <summary>
    /// Address for the Sentry CA service
    /// </summary>
    public string? SentryAddress { get; set; }

    private ushort? _metricsPort;

    /// <summary>
    /// The port that Dapr sends its metrics information to
    /// Must be greater than 0
    /// </summary>
    public ushort? MetricsPort
    {
        get => _metricsPort;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(MetricsPort)} must be greater than 0 .");

            _metricsPort = value;
        }
    }

    private ushort? _profilePort;

    /// <summary>
    /// The port for the profile server to listen on
    /// Must be greater than 0
    /// </summary>
    public ushort? ProfilePort
    {
        get => _profilePort;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(ProfilePort)} must be greater than 0 .");

            _profilePort = value;
        }
    }

    /// <summary>
    /// Path to a unix domain socket dir mount. If specified
    /// communication with the Dapr sidecar uses unix domain sockets for lower latency and greater throughput when compared to using TCP ports
    /// Not available on Windows OS
    /// </summary>
    public string? UnixDomainSocket { get; set; }

    private int? _daprMaxRequestSize;

    /// <summary>
    /// Max size of request body in MB.
    /// Must be greater than 0
    /// </summary>
    public int? DaprMaxRequestSize
    {
        get => _daprMaxRequestSize;
        set
        {
            if (value is <= 0)
                throw new NotSupportedException($"{nameof(DaprMaxRequestSize)} must be greater than 0 .");

            _daprMaxRequestSize = value;
        }
    }

    private int _heartBeatInterval = Constant.DEFAULT_HEARTBEAT_INTERVAL;

    /// <summary>
    /// Heartbeat detection interval, used to detect dapr status
    /// default: 5000 ms
    /// Must be greater than 0
    /// </summary>
    public int HeartBeatInterval
    {
        get => _heartBeatInterval;
        set
        {
            if (value < 0)
                throw new NotSupportedException($"{nameof(DaprMaxRequestSize)} must be greater than or equal to 0 .");

            _heartBeatInterval = value;
        }
    }

    /// <summary>
    /// Start the heartbeat check to ensure that the dapr program is active.
    /// When the heartbeat check is turned off, dapr will not start automatically after it exits abnormally.
    /// </summary>
    public bool EnableHeartBeat { get; set; } = true;

    public bool CreateNoWindow { get; set; } = true;
}
