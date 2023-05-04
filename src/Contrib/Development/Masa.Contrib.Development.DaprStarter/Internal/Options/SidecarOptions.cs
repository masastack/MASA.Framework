// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
internal class SidecarOptions : DaprOptionsBase
{
    /// <summary>
    /// The id for your application, used for service discovery
    /// Required, no blanks allowed
    /// </summary>
    public string AppId { get; }

    // ReSharper disable once InconsistentNaming
    public SidecarOptions(
        string appId,
        ushort? appPort,
        Protocol? appProtocol,
        bool? enableSsl,
        bool enableHeartBeat)
    {
        AppId = appId;
        AppPort = appPort;
        AppProtocol = appProtocol;
        EnableSsl = enableSsl;
        EnableHeartBeat = enableHeartBeat;
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

    public ushort GetAppPort()
    {
        MasaArgumentException.ThrowIfNull(AppPort);
        return AppPort.Value;
    }
}
