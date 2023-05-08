// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

public interface IDaprEnvironmentProvider
{
    ushort? GetHttpPort();

    ushort? GetGrpcPort();

    ushort? GetMetricsPort();

    /// <summary>
    /// Set the HttpPort environment variable
    /// When httpPort is greater than 0, return true
    /// </summary>
    /// <param name="httpPort"></param>
    /// <returns></returns>
    bool TrySetHttpPort(ushort? httpPort);

    /// <summary>
    /// Set the grpcPort environment variable
    /// When grpcPort is greater than 0, return true
    /// </summary>
    /// <param name="grpcPort"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    bool TrySetGrpcPort(ushort? grpcPort);

    /// <summary>
    /// Set the metricsPort environment variable
    /// When metricsPort is greater than 0, return true
    /// </summary>
    bool TrySetMetricsPort(ushort? metricsPort);

    void SetDaprAppId(string appId);
}
