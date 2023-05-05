// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public abstract class DaprProcessBase
{
    protected IDaprEnvironmentProvider DaprEnvironmentProvider { get; }

    private readonly IDaprProvider _daprProvider;

    private static readonly string[] HttpPortPatterns = { "HTTP Port: ([0-9]+)", "http server is running on port ([0-9]+)" };
    private static readonly string[] GrpcPortPatterns = { "gRPC Port: ([0-9]+)", "API gRPC server is running on port ([0-9]+)" };

    internal SidecarOptions? SuccessDaprOptions;

    protected DaprProcessBase(
        IDaprEnvironmentProvider daprEnvironmentProvider,
        IDaprProvider daprProvider)
    {
        DaprEnvironmentProvider = daprEnvironmentProvider;
        _daprProvider = daprProvider;
    }

    internal SidecarOptions ConvertToSidecarOptions(DaprOptions options)
    {
        var sidecarOptions = new SidecarOptions(
            _daprProvider.CompletionAppId(options.AppId),
            options.AppPort,
            options.AppProtocol,
            options.EnableSsl)
        {
            EnableHeartBeat = options.EnableHeartBeat,
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
            DaprHttpMaxRequestSize = options.DaprHttpMaxRequestSize,
            PlacementHostAddress = options.PlacementHostAddress,

            AllowedOrigins = options.AllowedOrigins,
            ControlPlaneAddress = options.ControlPlaneAddress,
            DaprHttpReadBufferSize = options.DaprHttpReadBufferSize,
            DaprInternalGrpcPort = options.DaprInternalGrpcPort,
            EnableApiLogging = options.EnableApiLogging,
            EnableMetrics = options.EnableMetrics,
            Mode = options.Mode,
            ResourcesPath = options.ResourcesPath,
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
            .Add("app-id", () => options.AppId)
            .Add("app-port", () => options.GetAppPort().ToString())
            .Add("app-protocol", () => options.AppProtocol!.Value.ToString().ToLower(), options.AppProtocol == null)
            .Add("app-ssl", () => "", options.EnableSsl != true)
            .Add("components-path", () => options.ComponentPath!, options.ComponentPath == null)
            .Add("app-max-concurrency", () => options.MaxConcurrency!.Value.ToString(), options.MaxConcurrency == null)
            .Add("config", () => options.Config!, options.Config == null)
            .Add("dapr-grpc-port", () => options.DaprGrpcPort!.Value.ToString(), !(options.DaprGrpcPort > 0))
            .Add("dapr-http-port", () => options.DaprHttpPort!.Value.ToString(), !(options.DaprHttpPort > 0))
            .Add("enable-profiling", () => options.EnableProfiling!.Value.ToString().ToLower(), options.EnableProfiling == null)
            .Add("log-level", () => options.LogLevel!.Value.ToString().ToLower(), options.LogLevel == null)
            .Add("sentry-address", () => options.SentryAddress!, options.SentryAddress == null)
            .Add("metrics-port", () => options.MetricsPort!.Value.ToString(), options.MetricsPort == null)
            .Add("profile-port", () => options.ProfilePort!.Value.ToString(), options.ProfilePort == null)
            .Add("unix-domain-socket", () => options.UnixDomainSocket!, options.UnixDomainSocket == null)
            .Add("dapr-http-max-request-size", () => options.DaprHttpMaxRequestSize!.Value.ToString(),
                options.DaprHttpMaxRequestSize == null)
            .Add("placement-host-address", () => options.PlacementHostAddress, options.PlacementHostAddress.IsNullOrWhiteSpace())
            .Add("allowed-origins", () => options.AllowedOrigins, options.AllowedOrigins.IsNullOrWhiteSpace())
            .Add("control-plane-address", () => options.ControlPlaneAddress, options.ControlPlaneAddress.IsNullOrWhiteSpace())
            .Add("dapr-http-read-buffer-size", () => options.DaprHttpReadBufferSize!.Value.ToString(),
                options.DaprHttpReadBufferSize == null)
            .Add("dapr-internal-grpc-port", () => options.DaprInternalGrpcPort!.Value.ToString(), options.DaprInternalGrpcPort == null)
            .Add("enable-api-logging", () => "", options.EnableApiLogging is not true)

            .Add("enable-metrics", () => "", options.EnableMetrics is not true)
            .Add("mode", () => options.Mode, options.Mode.IsNullOrWhiteSpace())
            .Add("resources-path", () => options.ResourcesPath, options.ResourcesPath.IsNullOrWhiteSpace());

        SuccessDaprOptions ??= options;
        return commandLineBuilder;
    }

    #region GetHttpPort、GetGrpcPort

    /// <summary>
    /// Get the HttpPort according to the output, but it is unreliable
    /// After the prompt information output by dapr is adjusted, it may cause the failure to obtain the port
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected static ushort? GetHttpPort(string data)
    {
        foreach (var pattern in HttpPortPatterns)
        {
            var port = GetPort(data, pattern);
            if (port is > 0)
                return port;
        }

        return null;
    }

    static ushort? GetPort(string data, string pattern)
    {
        ushort? port = null;
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        var match = regex.Matches(data);
        if (match.Count > 0)
        {
            port = ushort.Parse(match[0].Groups[1].ToString());
        }

        return port;
    }

    /// <summary>
    /// Get the gRpcPort according to the output, but it is unreliable
    /// After the prompt information output by dapr is adjusted, it may cause the failure to obtain the port
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected static ushort? GetGrpcPort(string data)
    {
        foreach (var pattern in GrpcPortPatterns)
        {
            var port = GetPort(data, pattern);
            if (port is > 0)
                return port;
        }

        return null;
    }

    #endregion
}
