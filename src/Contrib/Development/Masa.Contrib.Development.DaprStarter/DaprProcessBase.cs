// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public abstract class DaprProcessBase
{
    protected IDaprEnvironmentProvider DaprEnvironmentProvider { get; }

    private readonly IOptions<MasaAppConfigureOptions>? _masaAppConfigureOptions;

    private readonly IDaprProvider _daprProvider;

    /// <summary>
    /// Use after getting dapr AppId and global AppId fails
    /// </summary>
    private static readonly string DefaultAppId = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(
        ".",
        DaprStarterConstant.DEFAULT_APPID_DELIMITER);

    private const string HTTP_PORT_PATTERN = @"HTTP Port: ([0-9]+)";
    private const string GRPC_PORT_PATTERN = @"gRPC Port: ([0-9]+)";

    internal SidecarOptions? SuccessDaprOptions;

    protected DaprProcessBase(
        IDaprEnvironmentProvider daprEnvironmentProvider,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions,
        IDaprProvider daprProvider)
    {
        DaprEnvironmentProvider = daprEnvironmentProvider;
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _daprProvider = daprProvider;
    }

    internal SidecarOptions ConvertToSidecarOptions(DaprOptions options)
    {
        var sidecarOptions = new SidecarOptions(
            _daprProvider.CompletionAppId(options.AppId),
            options.AppPort,
            options.AppProtocol,
            options.EnableSsl,
            options.EnableHeartBeat)
        {
            HeartBeatInterval = options.HeartBeatInterval,
            CreateNoWindow = options.CreateNoWindow,
            MaxConcurrency = options.MaxConcurrency,
            Config = options.Config,
            ComponentPath = options.ComponentPath,
            EnableProfiling = options.EnableProfiling,
            LogLevel = options.LogLevel,
            SentryAddress = options.SentryAddress,
            MetricsPort = options.MetricsPort,
            ProfilePort = options.ProfilePort,
            UnixDomainSocket = options.UnixDomainSocket,
            DaprMaxRequestSize = options.DaprMaxRequestSize,
            PlacementHostAddress = options.PlacementHostAddress
        };
        sidecarOptions.TrySetHttpPort(options.DaprHttpPort ?? DaprEnvironmentProvider.GetHttpPort());
        sidecarOptions.TrySetGrpcPort(options.DaprGrpcPort ?? DaprEnvironmentProvider.GetGrpcPort());

        if (sidecarOptions.EnableDefaultPlacementHostAddress && sidecarOptions.PlacementHostAddress.IsNullOrWhiteSpace())
        {
            var port = Environment.OSVersion.Platform == PlatformID.Win32NT ? 6050 : 50005;
            sidecarOptions.PlacementHostAddress = $"127.0.0.1:{port}";
        }

        return sidecarOptions;
    }

    internal CommandLineBuilder CreateCommandLineBuilder(SidecarOptions options)
    {
        var commandLineBuilder = new CommandLineBuilder(DaprStarterConstant.DEFAULT_ARGUMENT_PREFIX);
        commandLineBuilder
            .Add("app-id", options.AppId)
            .Add("app-port", options.GetAppPort().ToString())
            .Add("app-protocol", options.AppProtocol?.ToString().ToLower() ?? string.Empty, options.AppProtocol == null)
            .Add("app-ssl", "", options.EnableSsl != true)
            .Add("components-path", options.ComponentPath ?? string.Empty, options.ComponentPath == null)
            .Add("app-max-concurrency", options.MaxConcurrency?.ToString() ?? string.Empty, options.MaxConcurrency == null)
            .Add("config", options.Config ?? string.Empty, options.Config == null)
            .Add("dapr-grpc-port", options.DaprGrpcPort?.ToString() ?? string.Empty, !(options.DaprGrpcPort > 0))
            .Add("dapr-http-port", options.DaprHttpPort?.ToString() ?? string.Empty, !(options.DaprHttpPort > 0))
            .Add("enable-profiling", options.EnableProfiling?.ToString().ToLower() ?? string.Empty, options.EnableProfiling == null)
            .Add("log-level", options.LogLevel?.ToString().ToLower() ?? string.Empty, options.LogLevel == null)
            .Add("sentry-address", options.SentryAddress ?? string.Empty, options.SentryAddress == null)
            .Add("metrics-port", options.MetricsPort?.ToString() ?? string.Empty, options.MetricsPort == null)
            .Add("profile-port", options.ProfilePort?.ToString() ?? string.Empty, options.ProfilePort == null)
            .Add("unix-domain-socket", options.UnixDomainSocket ?? string.Empty, options.UnixDomainSocket == null)
            .Add("dapr-http-max-request-size", options.DaprMaxRequestSize?.ToString() ?? string.Empty, options.DaprMaxRequestSize == null)
            .Add("placement-host-address", options.PlacementHostAddress, options.PlacementHostAddress.IsNullOrWhiteSpace());

        SuccessDaprOptions ??= options;
        return commandLineBuilder;
    }

    protected static ushort? GetHttpPort(string data)
    {
        ushort? httpPort = 0;
        var httpPortMatch = Regex.Matches(data, HTTP_PORT_PATTERN);
        if (httpPortMatch.Count > 0)
        {
            httpPort = ushort.Parse(httpPortMatch[0].Groups[1].ToString());
        }

        return httpPort;
    }

    protected static ushort? GetGrpcPort(string data)
    {
        ushort? grpcPort = 0;
        var grpcPortMatch = Regex.Matches(data, GRPC_PORT_PATTERN);
        if (grpcPortMatch.Count > 0)
        {
            grpcPort = ushort.Parse(grpcPortMatch[0].Groups[1].ToString());
        }

        return grpcPort;
    }
}
