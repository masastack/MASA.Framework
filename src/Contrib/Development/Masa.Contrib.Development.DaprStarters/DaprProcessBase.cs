// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters;

public abstract class DaprProcessBase
{
    private readonly IOptions<MasaAppConfigureOptions>? _masaAppConfigureOptions;

    /// <summary>
    /// Use after getting dapr AppId and global AppId fails
    /// </summary>
    private static readonly string _defaultAppId = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName().Name!.Replace(
        ".",
        Constant.DEFAULT_APPID_DELIMITER);

    private const string HTTP_PORT_PATTERN = @"HTTP Port: ([0-9]+)";
    private const string GRPC_PORT_PATTERN = @"gRPC Port: ([0-9]+)";

    internal DaprCoreOptions? SuccessDaprOptions;

    protected DaprProcessBase(IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions)
    {
        _masaAppConfigureOptions = masaAppConfigureOptions;
    }

    internal DaprCoreOptions ConvertTo(DaprOptions options)
    {
        var appId = options.AppId;
        if (appId.IsNullOrWhiteSpace())
            appId = _masaAppConfigureOptions?.Value.AppId;
        if (appId.IsNullOrWhiteSpace())
            appId = _defaultAppId;
        if (options.IsIncompleteAppId())
            appId = $"{appId}{options.AppIdDelimiter}{options.AppIdSuffix ?? NetworkUtils.GetPhysicalAddress()}";
        DaprCoreOptions
            dataOptions = new(
                appId,
                options.AppPort ?? throw new ArgumentNullException(nameof(options), $"{options.AppPort} must be greater than 0"),
                options.AppProtocol,
                options.EnableSsl,
                options.DaprGrpcPort,
                options.DaprHttpPort,
                options.EnableHeartBeat,
                options.HeartBeatInterval,
                options.CreateNoWindow)
            {
                MaxConcurrency = options.MaxConcurrency,
                Config = options.Config,
                ComponentPath = options.ComponentPath,
                EnableProfiling = options.EnableProfiling,
                Image = options.Image,
                LogLevel = options.LogLevel,
                PlacementHostAddress = options.PlacementHostAddress,
                SentryAddress = options.PlacementHostAddress,
                MetricsPort = options.MetricsPort,
                ProfilePort = options.ProfilePort,
                UnixDomainSocket = options.UnixDomainSocket,
                DaprMaxRequestSize = options.DaprMaxRequestSize
            };
        return dataOptions;
    }

    internal CommandLineBuilder CreateCommandLineBuilder(DaprCoreOptions options)
    {
        var commandLineBuilder = new CommandLineBuilder(Constant.DEFAULT_ARGUMENT_PREFIX);
        commandLineBuilder
            .Add("app-id", options.AppId)
            .Add("app-port", options.AppPort.ToString())
            .Add("app-protocol", options.AppProtocol?.ToString().ToLower() ?? string.Empty, options.AppProtocol == null)
            .Add("app-ssl", "", options.EnableSsl != true)
            .Add("components-path", options.ComponentPath ?? string.Empty, options.ComponentPath == null)
            .Add("app-max-concurrency", options.MaxConcurrency?.ToString() ?? string.Empty, options.MaxConcurrency == null)
            .Add("config", options.Config ?? string.Empty, options.Config == null)
            .Add("dapr-grpc-port", options.DaprGrpcPort?.ToString() ?? string.Empty, options.DaprGrpcPort == null)
            .Add("dapr-http-port", options.DaprHttpPort?.ToString() ?? string.Empty, options.DaprHttpPort == null)
            .Add("enable-profiling", options.EnableProfiling?.ToString().ToLower() ?? string.Empty, options.EnableProfiling == null)
            .Add("image", options.Image ?? string.Empty, options.Image == null)
            .Add("log-level", options.LogLevel?.ToString().ToLower() ?? string.Empty, options.LogLevel == null)
            .Add("placement-host-address", options.PlacementHostAddress ?? string.Empty, options.PlacementHostAddress == null)
            .Add("sentry-address", options.SentryAddress ?? string.Empty, options.SentryAddress == null)
            .Add("metrics-port", options.MetricsPort?.ToString() ?? string.Empty, options.MetricsPort == null)
            .Add("profile-port", options.ProfilePort?.ToString() ?? string.Empty, options.ProfilePort == null)
            .Add("unix-domain-socket", options.UnixDomainSocket ?? string.Empty, options.UnixDomainSocket == null)
            .Add("dapr-http-max-request-size", options.DaprMaxRequestSize?.ToString() ?? string.Empty, options.DaprMaxRequestSize == null);

        SuccessDaprOptions ??= options;
        return commandLineBuilder;
    }

    public bool CompleteDaprGrpcPortEnvironment(ushort daprGrpcPort)
        => EnvironmentUtils.TrySetEnvironmentVariable("DAPR_GRPC_PORT", daprGrpcPort.ToString());

    public bool CompleteDaprHttpPortEnvironment(ushort httpPort)
        => EnvironmentUtils.TrySetEnvironmentVariable("DAPR_HTTP_PORT", httpPort.ToString());

    public static ushort GetHttpPort(string data)
    {
        ushort httpPort = 0;
        var httpPortMatch = Regex.Matches(data, HTTP_PORT_PATTERN);
        if (httpPortMatch.Count > 0)
        {
            httpPort = ushort.Parse(httpPortMatch[0].Groups[1].ToString());
        }
        return httpPort;
    }

    public static ushort GetGrpcPort(string data)
    {
        ushort grpcPort = 0;
        var grpcPortMatch = Regex.Matches(data, GRPC_PORT_PATTERN);
        if (grpcPortMatch.Count > 0)
        {
            grpcPort = ushort.Parse(grpcPortMatch[0].Groups[1].ToString());
        }
        return grpcPort;
    }
}
