// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

public interface IDaprEnvironmentProvider
{
    ushort? GetHttpPort();

    ushort? GetGrpcPort();

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

    void SetHttpPort(ushort httpPort);

    // ReSharper disable once InconsistentNaming
    void SetGrpcPort(ushort grpcPort);

    // ReSharper disable once InconsistentNaming
    void CompleteDaprEnvironment(ushort? httpPort, ushort? grpcPort);
}
