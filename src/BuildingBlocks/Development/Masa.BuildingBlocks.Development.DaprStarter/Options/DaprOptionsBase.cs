// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Development.DaprStarter;

#pragma warning disable S3236
public abstract class DaprOptionsBase
{
    private ushort? _appPort;

    /// <summary>
    /// The port your application is listening on
    /// Required. Must be between 0-65535
    /// </summary>
    public ushort? AppPort
    {
        get => _appPort;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(AppPort));

            _appPort = value;
        }
    }

    /// <summary>
    /// The protocol (gRPC or HTTP) Dapr uses to talk to the application. Valid values are: http or grpc
    /// default: HTTP
    /// </summary>
    public Protocol? AppProtocol { get; set; } = Protocol.Http;

    /// <summary>
    /// Enable https when Dapr invokes the application
    /// Sets the URI scheme of the app to https and attempts an SSL connection
    /// </summary>
    public bool? EnableSsl { get; set; }

    /// <summary>
    /// The gRPC port for Dapr to listen on
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public virtual ushort? DaprGrpcPort { get; set; }

    /// <summary>
    /// The HTTP port for Dapr to listen on
    /// </summary>
    public virtual ushort? DaprHttpPort { get; set; }

    public virtual bool EnableHeartBeat { get; set; }

    public virtual int HeartBeatInterval { get; set; }

    public virtual bool CreateNoWindow { get; set; }

    /// <summary>
    /// The concurrency level of the application, otherwise is unlimited
    /// </summary>
    public virtual int? MaxConcurrency { get; set; }

    /// <summary>
    /// Dapr configuration file
    /// default: config.yaml
    /// The actual components-path is equal to Path.Combine(options.RootPath, options.Config)
    /// </summary>
    public string Config { get; set; }

    /// <summary>
    /// The path for components directory
    /// default: components
    /// The actual components-path is equal to Path.Combine(options.RootPath, options.ComponentPath)
    /// </summary>
    public string ComponentPath { get; set; }

    /// <summary>
    /// The root address of dapr configuration
    /// daprd, dapr runtime required component configuration path
    /// default：Linux/Mac: $HOME/.dapr/
    ///          Windows: %USERPROFILE%\.dapr\
    /// </summary>
    public string RootPath { get; set; }

    /// <summary>
    /// The path to the dapr directory
    /// defdult: Linux/Mac: /usr/local/bin
    ///          Windows: C:\dapr
    /// </summary>
    public string DaprRootPath { get; set; }

    /// <summary>
    /// Enable pprof profiling via an HTTP endpoint
    /// </summary>
    public bool? EnableProfiling { get; set; }

    /// <summary>
    /// The log verbosity. Valid values are: debug, info, warn, error, fatal, or panic
    /// default: info
    /// </summary>
    public LogLevel? LogLevel { get; set; }

    /// <summary>
    /// Address for the Sentry CA service
    /// </summary>
    public string? SentryAddress { get; set; }

    /// <summary>
    /// The port that Dapr sends its metrics information to
    /// </summary>
    public virtual ushort? MetricsPort { get; set; }

    /// <summary>
    /// The port for the profile server to listen on
    /// </summary>
    public virtual ushort? ProfilePort { get; set; }

    /// <summary>
    /// Path to a unix domain socket dir mount. If specified
    /// communication with the Dapr sidecar uses unix domain sockets for lower latency and greater throughput when compared to using TCP ports
    /// Not available on Windows OS
    /// </summary>
    public string? UnixDomainSocket { get; set; }
}
#pragma warning restore S3236
