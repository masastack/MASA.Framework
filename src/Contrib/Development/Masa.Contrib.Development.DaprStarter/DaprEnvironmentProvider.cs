// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

public class DaprEnvironmentProvider : IDaprEnvironmentProvider
{
    private const string GRPC_PORT = "DAPR_GRPC_PORT";

    private const string HTTP_PORT = "DAPR_HTTP_PORT";

    public ushort? GetHttpPort() => GetEnvironmentVariable(HTTP_PORT);

    public ushort? GetGrpcPort() => GetEnvironmentVariable(GRPC_PORT);

    private static ushort? GetEnvironmentVariable(string environment)
    {
        if (ushort.TryParse(Environment.GetEnvironmentVariable(environment), out ushort port))
            return port;

        return null;
    }

    public bool TrySetHttpPort(ushort? httpPort)
    {
        if (httpPort is > 0)
        {
            SetHttpPort(httpPort.Value);
            return true;
        }
        return false;
    }

    // ReSharper disable once InconsistentNaming
    public bool TrySetGrpcPort(ushort? grpcPort)
    {
        if (grpcPort is > 0)
        {
            SetGrpcPort(grpcPort.Value);
            return true;
        }
        return false;
    }

    public void SetHttpPort(ushort httpPort) => Environment.SetEnvironmentVariable(HTTP_PORT, httpPort.ToString());

    // ReSharper disable once InconsistentNaming
    public void SetGrpcPort(ushort grpcPort) => Environment.SetEnvironmentVariable(GRPC_PORT, grpcPort.ToString());

    // ReSharper disable once InconsistentNaming
    public void CompleteDaprEnvironment(ushort? httpPort, ushort? grpcPort)
    {
        if (grpcPort is > 0) SetGrpcPort(grpcPort.Value);

        if (httpPort is > 0) SetHttpPort(httpPort.Value);
    }
}
