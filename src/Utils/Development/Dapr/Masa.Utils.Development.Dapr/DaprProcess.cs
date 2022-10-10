// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr;

public class DaprProcess : IDaprProcess
{
    private readonly object _lock = new();

    private readonly IDaprProvider _daprProvider;
    private readonly IProcessProvider _processProvider;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly ILogger<DaprProcess>? _logger;
    private DaprProcessStatus Status { get; set; }
    private System.Timers.Timer? _heartBeatTimer;
    private DaprCoreOptions? _successDaprOptions;
    private int _retryTime;

    /// <summary>
    /// record whether dapr is initialized for the first time
    /// </summary>
    private bool _isFirst = true;

    public DaprProcess(IDaprProvider daprProvider, IProcessProvider processProvider, ILoggerFactory? loggerFactory)
    {
        _daprProvider = daprProvider;
        _processProvider = processProvider;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory?.CreateLogger<DaprProcess>();
    }

    public void Start(DaprOptions options, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            StartCore(GetDaprOptions(options), cancellationToken);
        }
    }

    private void StartCore(DaprCoreOptions options, CancellationToken cancellationToken = default)
    {
        UpdateStatus(DaprProcessStatus.Starting);
        var commandLineBuilder = Initialize(options, cancellationToken);
        StopCore(_successDaprOptions, cancellationToken);

        var utils = new ProcessUtils(_loggerFactory);

        utils.OutputDataReceived += delegate(object? sender, DataReceivedEventArgs args)
        {
            if (_isFirst)
            {
                CompleteDaprOptions(options, () => _isFirst = false);
            }
            DaprProcess_OutputDataReceived(sender, args);
        };
        utils.ErrorDataReceived += DaprProcess_ErrorDataReceived;
        utils.Exit += delegate
        {
            UpdateStatus(DaprProcessStatus.Stopped);
            _logger?.LogDebug("{Name} process has exited", Const.DEFAULT_FILE_NAME);
        };
        _retryTime = 0;
        utils.Run(Const.DEFAULT_FILE_NAME, $"run {commandLineBuilder}", options.CreateNoWindow);
        if (_heartBeatTimer == null && options.EnableHeartBeat)
        {
            _heartBeatTimer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = options.HeartBeatInterval
            };
            _heartBeatTimer.Elapsed += (sender, args) => HeartBeat(cancellationToken);
            _heartBeatTimer.Start();
        }
    }

    private static void DaprProcess_OutputDataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data == null) return;

        var dataSpan = e.Data.AsSpan();
        var levelStartIndex = e.Data.IndexOf("level=", StringComparison.Ordinal) + 6;
        var level = "information";
        if (levelStartIndex > 5)
        {
            var levelLength = dataSpan.Slice(levelStartIndex).IndexOf(' ');
            level = dataSpan.Slice(levelStartIndex, levelLength).ToString();
        }

        var color = Console.ForegroundColor;
        switch (level)
        {
            case "warning":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "error":
            case "critical":
            case "fatal":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                break;
        }

        Console.WriteLine(e.Data);
        Console.ForegroundColor = color;
    }

    private static void DaprProcess_ErrorDataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data == null) return;

        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Data);
        Console.ForegroundColor = color;
    }

    public void Stop(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            StopCore(_successDaprOptions, cancellationToken);
        }
    }

    private void StopCore(DaprCoreOptions? options, CancellationToken cancellationToken = default)
    {
        if (options != null)
        {
            _daprProvider.DaprStop(options.AppId);
            if (options.DaprHttpPort != null)
                CheckPortAndKill(options.DaprHttpPort.Value);
            if (options.DaprGrpcPort != null)
                CheckPortAndKill(options.DaprGrpcPort.Value);
        }
    }

    /// <summary>
    /// Refresh the dapr configuration, the source dapr process will be killed and the new dapr process will be restarted
    /// todo: At present, there are no restrictions on HttpPort and GrpcPort, but if the configuration update changes HttpPort and GrpcPort, the port obtained by DaprClient will be inconsistent with the actual operation, which needs to be adjusted later.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    public void Refresh(DaprOptions options, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _logger?.LogDebug("Dapr configuration refresh, appid is {appid}, please wait...", _successDaprOptions!.AppId);

            if (_successDaprOptions != null)
            {
                UpdateStatus(DaprProcessStatus.Restarting);
                _logger?.LogDebug("Dapr configuration refresh, appid is {appid}, closing dapr, please wait...",
                    _successDaprOptions!.AppId);
                StopCore(_successDaprOptions, cancellationToken);
            }

            _isFirst = true;
            _successDaprOptions = null;
            _logger?.LogDebug("Dapr configuration refresh, appid is {appid}, restarting dapr, please wait...", options.AppId);
            StartCore(GetDaprOptions(options), cancellationToken);
        }
    }

    private void CheckPortAndKill(ushort port)
    {
        if (!_processProvider.IsAvailablePorts(port))
        {
            var pIdList = _processProvider.GetPidByPort(port);
            foreach (var pId in pIdList)
            {
                var process = _processProvider.GetProcess(pId);
                _logger?.LogWarning("Port {Port} is used, PId: {PId}, PName: {PName} , Process has been killed by {Name}",
                    port,
                    pId,
                    process.Name,
                    nameof(Dapr));
                process.Kill();
            }
        }
    }

    private void HeartBeat(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (!_daprProvider.IsExist(_successDaprOptions!.AppId))
            {
                if (Status == DaprProcessStatus.Started || Status == DaprProcessStatus.Stopped)
                {
                    _logger?.LogWarning("Dapr stopped, restarting, please wait...");
                    StartCore(_successDaprOptions, cancellationToken);
                }
                else if (Status == DaprProcessStatus.Starting)
                {
                    if (_retryTime < Const.DEFAULT_RETRY_TIME)
                    {
                        _retryTime++;
                        _logger?.LogDebug("Dapr is not started: The {retries}th heartbeat check. AppId is {AppId}",
                            _retryTime,
                            _successDaprOptions.AppId);
                    }
                    else
                    {
                        _logger?.LogWarning(
                            "Dapr is not started: The {retries}th heartbeat check. Dapr stopped, restarting, please wait...",
                            _retryTime + 1);
                        StartCore(_successDaprOptions, cancellationToken);
                    }
                }
                else
                {
                    _logger?.LogWarning("Dapr is restarting, the current state is {State}, please wait...", Status);
                }
            }
            else
            {
                _retryTime = 0;
                UpdateStatus(DaprProcessStatus.Started);
            }
        }
    }

    private DaprCoreOptions GetDaprOptions(DaprOptions options)
    {
        string appId = options.GetAppId();
        ushort appPort = options.GetAppPort();
        DaprCoreOptions dataOptions = new(
            appId,
            appPort,
            options.AppProtocol,
            options.EnableSsl,
            options.DaprGrpcPort,
            options.DaprHttpPort,
            options.EnableHeartBeat,
            options.HeartBeatInterval!.Value,
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
        dataOptions.OutputDataReceived += options.Output;
        return dataOptions;
    }

    private CommandLineBuilder Initialize(DaprCoreOptions options, CancellationToken cancellationToken)
    {
        var commandLineBuilder = new CommandLineBuilder(Const.DEFAULT_ARGUMENT_PREFIX);
        commandLineBuilder
            .Add("app-id", options.AppId)
            .Add("app-port", options.AppPort.ToString())
            .Add("app-protocol", options.AppProtocol?.ToString().ToLower() ?? string.Empty, options.AppProtocol == null)
            .Add("app-ssl", options.EnableSsl?.ToString().ToLower() ?? "", options.EnableSsl == null)
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

        _successDaprOptions ??= options;
        return commandLineBuilder;
    }

    /// <summary>
    /// Improve the information of HttpPort and GrpcPort successfully configured.
    /// When Port is specified or Dapr is closed for other reasons after startup, the HttpPort and GrpcPort are the same as the Port assigned at the first startup.
    /// </summary>
    private void CompleteDaprOptions(DaprCoreOptions options, Action action)
    {
        int retry = 0;
        if (_successDaprOptions!.DaprHttpPort == null || _successDaprOptions.DaprGrpcPort == null)
        {
            again:
            var daprList = _daprProvider.GetDaprList(_successDaprOptions.AppId);
            if (daprList.Any())
            {
                var currentDapr = daprList.FirstOrDefault()!;
                _successDaprOptions.SetPort(currentDapr.HttpPort, currentDapr.GrpcPort);
            }
            else
            {
                if (retry < 3)
                {
                    retry++;
                    goto again;
                }
                _logger?.LogWarning("Dapr failed to start, appid is {Appid}", _successDaprOptions!.AppId);
                return;
            }
        }

        string daprHttpPort = _successDaprOptions.DaprHttpPort.ToString()!;
        string daprGrpcPort = _successDaprOptions.DaprGrpcPort.ToString()!;
        CompleteDaprEnvironment(daprHttpPort, daprGrpcPort, out bool isChange);
        action.Invoke();
        if (isChange)
        {
            options.Output(Const.CHANGE_DAPR_ENVIRONMENT_VARIABLE,
                $"update environment variables, DaprHttpPort: {daprHttpPort}, DAPR_GRPC_PORT: {daprGrpcPort}");
        }
    }

    private void UpdateStatus(DaprProcessStatus status)
    {
        if (status != Status)
        {
            _logger?.LogDebug($"Dapr Process Status Change: {Status} -> {status}");
            Status = status;
        }
    }

    private static void CompleteDaprEnvironment(string daprHttpPort, string daprGrpcPort, out bool isChange)
    {
        EnvironmentExtensions.TryAdd("DAPR_GRPC_PORT", () => daprGrpcPort, out bool gRpcPortIsExist);
        EnvironmentExtensions.TryAdd("DAPR_HTTP_PORT", () => daprHttpPort, out bool httpPortIsExist);
        isChange = !gRpcPortIsExist || !httpPortIsExist;
    }

    public void Dispose() => Stop();
}
