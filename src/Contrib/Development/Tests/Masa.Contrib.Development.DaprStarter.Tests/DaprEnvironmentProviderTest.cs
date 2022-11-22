// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.Tests;

[TestClass]
public class DaprEnvironmentProviderTest
{
    private const string GRPC_PORT = "DAPR_GRPC_PORT";

    private const string HTTP_PORT = "DAPR_HTTP_PORT";
    private readonly DaprEnvironmentProvider _daprEnvironmentProvider;

    public DaprEnvironmentProviderTest()
    {
        _daprEnvironmentProvider = new DaprEnvironmentProvider();
        Environment.SetEnvironmentVariable(HTTP_PORT, null);
        Environment.SetEnvironmentVariable(GRPC_PORT, null);
    }

    [TestMethod]
    public void TestTrySetHttpPort()
    {
        ushort? httpPort = null;
        var result = _daprEnvironmentProvider.TrySetHttpPort(httpPort);
        Assert.AreEqual(false, result);

        httpPort = _daprEnvironmentProvider.GetHttpPort();
        Assert.AreEqual(null, httpPort);

        httpPort = 10;
        result = _daprEnvironmentProvider.TrySetHttpPort(httpPort);
        Assert.AreEqual(true, result);

        var port = _daprEnvironmentProvider.GetHttpPort();
        Assert.AreEqual(httpPort, port);
    }

    [TestMethod]
    // ReSharper disable once InconsistentNaming
    public void TestTrySetGrpcPort()
    {
        // ReSharper disable once InconsistentNaming
        ushort? grpcPort = null;
        var result = _daprEnvironmentProvider.TrySetGrpcPort(grpcPort);
        Assert.AreEqual(false, result);

        grpcPort = _daprEnvironmentProvider.GetGrpcPort();
        Assert.AreEqual(null, grpcPort);

        grpcPort = 10;
        result = _daprEnvironmentProvider.TrySetGrpcPort(grpcPort);
        Assert.AreEqual(true, result);

        var port = _daprEnvironmentProvider.GetGrpcPort();
        Assert.AreEqual(grpcPort, port);
    }

    [TestMethod]
    // ReSharper disable once InconsistentNaming
    public void TestCompleteDaprEnvironment()
    {
        // ReSharper disable once InconsistentNaming
        ushort? grpcPort = null;
        ushort? httpPort = null;
        _daprEnvironmentProvider.CompleteDaprEnvironment(httpPort, grpcPort);

        grpcPort = _daprEnvironmentProvider.GetGrpcPort();
        Assert.AreEqual(null, grpcPort);

        httpPort = _daprEnvironmentProvider.GetHttpPort();
        Assert.AreEqual(null, httpPort);

        httpPort = 9;
        grpcPort = 10;

        _daprEnvironmentProvider.CompleteDaprEnvironment(httpPort, grpcPort);

        httpPort = _daprEnvironmentProvider.GetHttpPort();
        Assert.AreEqual((ushort)9, httpPort);

        grpcPort = _daprEnvironmentProvider.GetGrpcPort();
        Assert.AreEqual((ushort)10, grpcPort);
    }
}
