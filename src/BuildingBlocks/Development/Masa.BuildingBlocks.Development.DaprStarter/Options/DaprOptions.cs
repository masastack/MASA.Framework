// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Development.DaprStarter;

/// <summary>
/// dapr startup configuration information
/// When the specified attribute is configured as null, the default value of the parameter is subject to the default value of dapr of the current version
/// </summary>
public class DaprOptions: DaprOptionsBase
{
    /// <summary>
    /// The id for your application, used for service discovery
    /// </summary>
    public string? AppId { get; set; }

    private string _appIdDelimiter = DaprStarterConstant.DEFAULT_APPID_DELIMITER;

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
            MasaArgumentException.ThrowIfContain(value, ".", nameof(AppIdDelimiter));

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
            MasaArgumentException.ThrowIfContain(value, ".", nameof(AppIdSuffix));

            _appIdSuffix = value;
        }
    }

    /// <summary>
    /// Whether to disable AppIdSuffix
    /// default: false
    /// </summary>
    public bool DisableAppIdSuffix { get; set; }

    private int? _maxConcurrency;

    /// <summary>
    /// The concurrency level of the application, otherwise is unlimited
    /// Must be greater than 0
    /// </summary>
    public override int? MaxConcurrency
    {
        get => _maxConcurrency;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(MaxConcurrency));

            _maxConcurrency = value;
        }
    }

    private ushort? _daprGrpcPort;

    /// <summary>
    /// The gRPC port for Dapr to listen on
    /// Must be greater than 0
    /// </summary>
    public override ushort? DaprGrpcPort
    {
        get => _daprGrpcPort;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(DaprGrpcPort));

            _daprGrpcPort = value;
        }
    }

    private ushort? _daprHttpPort;

    /// <summary>
    /// The HTTP port for Dapr to listen on
    /// Must be greater than 0
    /// </summary>
    public override ushort? DaprHttpPort
    {
        get => _daprHttpPort;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(DaprHttpPort));

            _daprHttpPort = value;
        }
    }

    private ushort? _metricsPort;

    /// <summary>
    /// The port that Dapr sends its metrics information to
    /// Must be greater than 0
    /// </summary>
    public override ushort? MetricsPort
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
    public override ushort? ProfilePort
    {
        get => _profilePort;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(ProfilePort));

            _profilePort = value;
        }
    }

    private int? _daprMaxRequestSize;

    /// <summary>
    /// Max size of request body in MB.
    /// Must be greater than 0
    /// </summary>
    public override int? DaprMaxRequestSize
    {
        get => _daprMaxRequestSize;
        set
        {
            if (value != null)
                MasaArgumentException.ThrowIfLessThanOrEqual(value.Value, (ushort)0, nameof(DaprMaxRequestSize));

            _daprMaxRequestSize = value;
        }
    }

    private int _heartBeatInterval = DaprStarterConstant.DEFAULT_HEARTBEAT_INTERVAL;

    /// <summary>
    /// Heartbeat detection interval, used to detect dapr status
    /// default: 5000 ms
    /// Must be greater than 0
    /// </summary>
    public override int HeartBeatInterval
    {
        get => _heartBeatInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, (ushort)0, nameof(HeartBeatInterval));

            _heartBeatInterval = value;
        }
    }

    public string PlacementHostAddress { get; set; }

    public bool IsIncompleteAppId()
    {
        return !DisableAppIdSuffix && (AppIdSuffix == null || AppIdSuffix.Trim() != string.Empty);
    }
}
