﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

#pragma warning disable S3236
[ExcludeFromCodeCoverage]
internal class SidecarOptions : DaprOptionsBase
{
    /// <summary>
    /// The id for your application, used for service discovery
    /// Required, no blanks allowed
    /// </summary>
    public string AppId { get; }

    /// <summary>
    /// Whether to use the default placement host address when no PlacementHostAddress is specified
    /// default: true
    /// </summary>
    public bool EnableDefaultPlacementHostAddress { get; set; } = true;

    public string PlacementHostAddress { get; set; }

    /// <summary>
    /// Allowed HTTP origins (default "*")
    /// </summary>
    public string AllowedOrigins { get; set; }

    /// <summary>
    /// Address for a Dapr control plane
    /// </summary>
    public string ControlPlaneAddress { get; set; }

    /// <summary>
    /// Increasing max size of request body in MB to handle uploading of big files (default 4)
    /// </summary>
    public int? DaprHttpMaxRequestSize { get; set; }

    /// <summary>
    /// Increasing max size of read buffer in KB to handle sending multi-KB headers (default 4)
    /// </summary>
    public int? DaprHttpReadBufferSize { get; set; }

    /// <summary>
    /// gRPC port for the Dapr Internal API to listen on.
    /// </summary>
    public int? DaprInternalGrpcPort { get; set; }

    /// <summary>
    /// Enable API logging for API calls
    /// </summary>
    public bool? EnableApiLogging { get; set; }

    /// <summary>
    /// Enable prometheus metric (default true)
    /// </summary>
    public bool? EnableMetrics { get; set; }

    /// <summary>
    /// Runtime mode for Dapr (default "standalone")
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// Extended parameters, used to supplement unsupported parameters
    /// </summary>
    public string ExtendedParameter { get; set; }

    // ReSharper disable once InconsistentNaming
    public SidecarOptions(
        string appId,
        ushort? appPort,
        Protocol? appProtocol,
        bool? enableSsl)
    {
        AppId = appId;
        AppPort = appPort;
        AppProtocol = appProtocol;
        EnableSsl = enableSsl;
    }

    public bool TrySetHttpPort(ushort? httpPort)
    {
        if (DaprHttpPort == null && httpPort is > 0)
        {
            DaprHttpPort = httpPort;
            return true;
        }

        return false;
    }

    // ReSharper disable once InconsistentNaming
    public bool TrySetGrpcPort(ushort? grpcPort)
    {
        if (DaprGrpcPort == null && grpcPort is > 0)
        {
            DaprGrpcPort = grpcPort;
            return true;
        }

        return false;
    }

    public bool TrySetProfilePort(ushort? profilePort)
    {
        if (ProfilePort == null && profilePort is > 0)
        {
            ProfilePort = profilePort;
            return true;
        }

        return false;
    }

    public bool TrySetMetricsPort(ushort? metricsPort)
    {
        if (MetricsPort == null && metricsPort is > 0)
        {
            MetricsPort = metricsPort;
            return true;
        }

        return false;
    }

    public ushort GetAppPort()
    {
        MasaArgumentException.ThrowIfNull(AppPort);
        return AppPort.Value;
    }

    public override int GetHashCode()
    {
        return GetContent().Aggregate(0, HashCode.Combine);
    }

    public override bool Equals(object? obj)
    {
        if (this is null ^ obj is null) return false;

        if (obj is SidecarOptions other)
        {
            return other.GetContent().SequenceEqual(GetContent());
        }

        return false;
    }

    private IEnumerable<object?> GetContent()
    {
        return new List<object?>
        {
            GetAppPort(),
            AppProtocol,
            EnableSsl,
            DaprGrpcPort,
            DaprHttpPort,
            EnableHeartBeat,
            HeartBeatInterval,
            CreateNoWindow,
            MaxConcurrency,
            Config,
            ComponentPath,
            EnableProfiling,
            LogLevel,
            SentryAddress,
            MetricsPort,
            ProfilePort,
            UnixDomainSocket,
            DaprHttpReadBufferSize,
            AppId,
            EnableDefaultPlacementHostAddress,
            PlacementHostAddress,
            AllowedOrigins,
            ControlPlaneAddress,
            DaprHttpMaxRequestSize,
            DaprInternalGrpcPort,
            EnableApiLogging,
            EnableMetrics,
            Mode,
            ExtendedParameter
        };
    }
}
#pragma warning restore S3236
