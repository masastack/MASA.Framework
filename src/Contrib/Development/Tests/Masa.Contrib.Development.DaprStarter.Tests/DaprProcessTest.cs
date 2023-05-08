// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter.Tests;

[TestClass]
public class DaprProcessTest
{
    [DataRow(true, "AppId", "inputPlacementHostAddress", "inputRootPath", "inputDaprRootPath", "inputComponentPath", "inputConfig")]
    [DataRow(true, "AppId", "", "", "", "", "")]
    [DataRow(false, "AppId-AppIdSuffix", "inputPlacementHostAddress", "inputRootPath", "inputDaprRootPath", "inputComponentPath", "inputConfig")]
    [DataRow(false, "AppId-AppIdSuffix", "", "", "", "", "")]
    [DataTestMethod]
    public void TestConvertToSidecarOptions(
        bool disableAppIdSuffix,
        string expectedAppId,
        string inputPlacementHostAddress,
        string inputRootPath,
        string inputDaprRootPath,
        string inputComponentPath,
        string inputConfig)
    {
        var daprOptions = new DaprOptions()
        {
            AppPort = 5000,
            AppIdSuffix = "AppIdSuffix",
            AppProtocol = Protocol.GRpc,
            EnableSsl = true,
            DaprGrpcPort = 5001,
            DaprHttpPort = 3501,
            EnableHeartBeat = false,
            HeartBeatInterval = 30,
            CreateNoWindow = true,
            MaxConcurrency = 1,
            Config = inputConfig,
            ComponentPath = inputComponentPath,
            RootPath = inputRootPath,
            DaprRootPath = inputDaprRootPath,
            EnableProfiling = true,
            LogLevel = LogLevel.Error,
            SentryAddress = "SentryAddress",
            MetricsPort = 9000,
            ProfilePort = 1,
            UnixDomainSocket = "UnixDomainSocket",
            AppId = "AppId",
            AppIdDelimiter = "-",
            DisableAppIdSuffix = disableAppIdSuffix,
            DaprHttpMaxRequestSize = 1,
            PlacementHostAddress = inputPlacementHostAddress,
            AllowedOrigins = "AllowedOrigins",
            ControlPlaneAddress = "ControlPlaneAddress",
            DaprHttpReadBufferSize = 1,
            DaprInternalGrpcPort = 1,
            EnableApiLogging = true,
            EnableMetrics = true,
            Mode = "Mode",
            ExtendedParameter = "ExtendedParameter"
        };
        Mock<IDaprEnvironmentProvider> daprEnvironmentProvider = new();
        Mock<IOptionsMonitor<DaprOptions>> optionsMonitor = new();

        var customDaprProcess = new CustomDaprProcess(daprEnvironmentProvider.Object, new DefaultDaprProvider(), optionsMonitor.Object);
        var sidecarOptions = customDaprProcess.ConvertToSidecarOptions(daprOptions);
        Assert.AreEqual(expectedAppId, sidecarOptions.AppId);
        Assert.AreEqual(daprOptions.AppPort, sidecarOptions.AppPort);
        Assert.AreEqual(daprOptions.AppProtocol, sidecarOptions.AppProtocol);
        Assert.AreEqual(daprOptions.EnableSsl, sidecarOptions.EnableSsl);
        Assert.AreEqual(daprOptions.DaprGrpcPort, sidecarOptions.DaprGrpcPort);
        Assert.AreEqual(daprOptions.DaprHttpPort, sidecarOptions.DaprHttpPort);
        Assert.AreEqual(daprOptions.EnableHeartBeat, sidecarOptions.EnableHeartBeat);
        Assert.AreEqual(daprOptions.HeartBeatInterval, sidecarOptions.HeartBeatInterval);
        Assert.AreEqual(daprOptions.CreateNoWindow, sidecarOptions.CreateNoWindow);
        Assert.AreEqual(daprOptions.MaxConcurrency, sidecarOptions.MaxConcurrency);
        Assert.AreEqual(GetConfig(inputConfig), sidecarOptions.Config);
        Assert.AreEqual(GetComponentPath(inputComponentPath), sidecarOptions.ComponentPath);
        Assert.AreEqual(GetRootPath(inputRootPath), sidecarOptions.RootPath);

        Assert.AreEqual(GetDaprRootPath(inputDaprRootPath), sidecarOptions.DaprRootPath);
        Assert.AreEqual(daprOptions.EnableProfiling, sidecarOptions.EnableProfiling);
        Assert.AreEqual(daprOptions.LogLevel, sidecarOptions.LogLevel);
        Assert.AreEqual(daprOptions.SentryAddress, sidecarOptions.SentryAddress);
        Assert.AreEqual(daprOptions.MetricsPort, sidecarOptions.MetricsPort);
        Assert.AreEqual(daprOptions.ProfilePort, sidecarOptions.ProfilePort);
        Assert.AreEqual(daprOptions.UnixDomainSocket, sidecarOptions.UnixDomainSocket);
        Assert.AreEqual(daprOptions.EnableDefaultPlacementHostAddress, sidecarOptions.EnableDefaultPlacementHostAddress);

        Assert.AreEqual(GetPlacementHostAddress(inputPlacementHostAddress), sidecarOptions.PlacementHostAddress);
        Assert.AreEqual(daprOptions.AllowedOrigins, sidecarOptions.AllowedOrigins);
        Assert.AreEqual(daprOptions.ControlPlaneAddress, sidecarOptions.ControlPlaneAddress);
        Assert.AreEqual(daprOptions.DaprHttpMaxRequestSize, sidecarOptions.DaprHttpMaxRequestSize);
        Assert.AreEqual(daprOptions.DaprHttpReadBufferSize, sidecarOptions.DaprHttpReadBufferSize);
        Assert.AreEqual(daprOptions.DaprInternalGrpcPort, sidecarOptions.DaprInternalGrpcPort);
        Assert.AreEqual(daprOptions.EnableApiLogging, sidecarOptions.EnableApiLogging);
        Assert.AreEqual(daprOptions.EnableMetrics, sidecarOptions.EnableMetrics);
        Assert.AreEqual(daprOptions.Mode, sidecarOptions.Mode);
        Assert.AreEqual(daprOptions.ExtendedParameter, sidecarOptions.ExtendedParameter);

        string GetPlacementHostAddress(string placementHostAddress)
        {
            if (!placementHostAddress.IsNullOrWhiteSpace())
                return placementHostAddress;

            var port = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 6050 : 50005;
            return $"127.0.0.1:{port}";
        }

        string GetRootPath(string rootPath)
        {
            if (rootPath.IsNullOrWhiteSpace())
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dapr");

            return rootPath;
        }

        string GetDaprRootPath(string daprRootPath)
        {
            if (daprRootPath.IsNullOrWhiteSpace())
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:\\dapr" : "/usr/local/bin";

            return daprRootPath;
        }

        string GetComponentPath(string componentPath)
        {
            if (!componentPath.IsNullOrWhiteSpace())
                return componentPath;

            return "components";
        }

        string GetConfig(string config)
        {
            if (!config.IsNullOrWhiteSpace())
                return config;

            return "config.yaml";
        }
    }
}
