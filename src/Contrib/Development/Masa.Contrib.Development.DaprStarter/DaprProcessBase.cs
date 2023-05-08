// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public abstract class DaprProcessBase
{
    protected IDaprEnvironmentProvider DaprEnvironmentProvider { get; }

    private readonly IDaprProvider _daprProvider;

    private static readonly string[] HttpPortPatterns = { "http server is running on port ([0-9]+)" };
    private static readonly string[] GrpcPortPatterns = { "API gRPC server is running on port ([0-9]+)" };
    private static readonly string UserFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    internal SidecarOptions? SuccessDaprOptions;
    protected readonly IOptionsMonitor<DaprOptions> DaprOptions;
    private static string? _defaultDaprFileName;
    private static string? _defaultSidecarFileName;

    /// <summary>
    /// record whether dapr is initialized for the first time
    /// </summary>
    protected bool IsFirst = true;

    protected DaprProcessBase(
        IDaprEnvironmentProvider daprEnvironmentProvider,
        IDaprProvider daprProvider,
        IOptionsMonitor<DaprOptions> daprOptions)
    {
        DaprEnvironmentProvider = daprEnvironmentProvider;
        _daprProvider = daprProvider;
        DaprOptions = daprOptions;
    }

    internal SidecarOptions ConvertToSidecarOptions(DaprOptions options)
    {
        var daprAppIdByEnvironment = DaprEnvironmentProvider.GetDaprAppId();
        var daprAppId = daprAppIdByEnvironment.IsNullOrWhiteSpace() ?
            _daprProvider.CompletionAppId(options.AppId, options.DisableAppIdSuffix, options.AppIdSuffix, options.AppIdDelimiter) :
            daprAppIdByEnvironment;
        var sidecarOptions = new SidecarOptions(
            daprAppId,
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
            RootPath = options.RootPath,
            DaprRootPath = options.DaprRootPath,
            ExtendedParameter = options.ExtendedParameter
        };
        sidecarOptions.TrySetHttpPort(options.DaprHttpPort ?? DaprEnvironmentProvider.GetHttpPort());
        sidecarOptions.TrySetGrpcPort(options.DaprGrpcPort ?? DaprEnvironmentProvider.GetGrpcPort());
        sidecarOptions.TrySetMetricsPort(options.MetricsPort ?? DaprEnvironmentProvider.GetMetricsPort());

        if (sidecarOptions.EnableDefaultPlacementHostAddress && sidecarOptions.PlacementHostAddress.IsNullOrWhiteSpace())
        {
            var port = Environment.OSVersion.Platform == PlatformID.Win32NT ? 6050 : 50005;
            sidecarOptions.PlacementHostAddress = $"127.0.0.1:{port}";
        }

        if (sidecarOptions.RootPath.IsNullOrWhiteSpace())
        {
            sidecarOptions.RootPath = Path.Combine(UserFilePath, ".dapr");
        }

        if (sidecarOptions.DaprRootPath.IsNullOrWhiteSpace())
        {
            sidecarOptions.DaprRootPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:\\dapr" : "/usr/local/bin";
        }

        if (sidecarOptions.ComponentPath.IsNullOrWhiteSpace())
        {
            sidecarOptions.ComponentPath = "components";
        }

        if (sidecarOptions.Config.IsNullOrWhiteSpace())
        {
            sidecarOptions.Config = "config.yaml";
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
            .Add("components-path", () => Path.Combine(options.RootPath, options.ComponentPath))
            .Add("app-max-concurrency", () => options.MaxConcurrency!.Value.ToString(), options.MaxConcurrency == null)
            .Add("config", () => Path.Combine(options.RootPath, options.Config))
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
            .Add(options.ExtendedParameter, () => "", options.ExtendedParameter.IsNullOrWhiteSpace());

        if (!IsFirst)
            return commandLineBuilder;

        SetDefaultFileName(options.DaprRootPath, options.RootPath);
        SuccessDaprOptions = options;

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
        var regex = new Regex(pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
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

    static string GetFileName(string fileName) => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"{fileName}.exe" : fileName;

    protected static string GetDefaultDaprFileName() => _defaultDaprFileName!;

    protected static string GetDefaultSidecarFileName() => _defaultSidecarFileName!;

    private static void SetDefaultFileName(string daprRootPath, string sidecarRootPath)
    {
        _defaultDaprFileName = Path.Combine(daprRootPath, GetFileName(DaprStarterConstant.DEFAULT_DAPR_FILE_NAME));
        _defaultSidecarFileName = Path.Combine(sidecarRootPath, "bin", GetFileName(DaprStarterConstant.DEFAULT_FILE_NAME));
    }
}
