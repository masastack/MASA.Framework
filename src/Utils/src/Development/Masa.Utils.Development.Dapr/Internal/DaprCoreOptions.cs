﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.Internal;

internal class DaprCoreOptions
{
    /// <summary>
    /// The id for your application, used for service discovery
    /// Required, no blanks allowed
    /// </summary>
    public string AppId { get; }

    /// <summary>
    /// The port your application is listening on
    /// </summary>
    public ushort AppPort { get; }

    /// <summary>
    /// The protocol (gRPC or HTTP) Dapr uses to talk to the application. Valid values are: http or grpc
    /// </summary>
    public Protocol? AppProtocol { get; }

    /// <summary>
    /// Enable https when Dapr invokes the application
    /// </summary>
    public bool? EnableSsl { get; }

    /// <summary>
    /// The gRPC port for Dapr to listen on
    /// </summary>
    public ushort? DaprGrpcPort { get; private set; }

    /// <summary>
    /// The HTTP port for Dapr to listen on
    /// </summary>
    public ushort? DaprHttpPort { get; private set; }

    public bool EnableHeartBeat { get; private set; }

    public int HeartBeatInterval { get; }

    public bool CreateNoWindow { get; } = true;

    /// <summary>
    /// The concurrency level of the application, otherwise is unlimited
    /// </summary>
    public int? MaxConcurrency { get; set; }

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

    /// <summary>
    /// The port that Dapr sends its metrics information to
    /// </summary>
    public ushort? MetricsPort { get; set; }

    /// <summary>
    /// The port for the profile server to listen on
    /// </summary>
    public ushort? ProfilePort { get; set; }

    /// <summary>
    /// Path to a unix domain socket dir mount. If specified
    /// communication with the Dapr sidecar uses unix domain sockets for lower latency and greater throughput when compared to using TCP ports
    /// Not available on Windows OS
    /// </summary>
    public string? UnixDomainSocket { get; set; }

    /// <summary>
    /// Max size of request body in MB.
    /// </summary>
    public int? DaprMaxRequestSize { get; set; }

    public event DaprEventHandler? OutputDataReceived;

    public DaprCoreOptions(
        string appId,
        ushort appPort,
        Protocol? appProtocol,
        bool? enableSsl,
        ushort? daprGrpcPort,
        ushort? daprHttpPort,
        bool enableHeartBeat,
        int heartBeatInterval,
        bool createNoWindow)
    {
        AppId = appId;
        AppPort = appPort;
        AppProtocol = appProtocol;
        EnableSsl = enableSsl;
        DaprGrpcPort = daprGrpcPort;
        DaprHttpPort = daprHttpPort;
        EnableHeartBeat = enableHeartBeat;
        HeartBeatInterval = heartBeatInterval;
        CreateNoWindow = createNoWindow;
    }

    public void SetPort(ushort httpPort, ushort rpcPort)
    {
        DaprHttpPort ??= httpPort;
        DaprGrpcPort ??= rpcPort;
    }

    public void Output(string type, string data) => OutputDataReceived?.Invoke(type, data);
}
