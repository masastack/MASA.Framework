// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public class DaprProcess : DaprProcessBase, IDaprProcess
{
    private readonly object _lock = new();

    private readonly IDaprProcessProvider _daprProcessProvider;
    private readonly IProcessProvider _processProvider;
    private readonly ILogger<DaprProcess>? _logger;
    private DaprProcessStatus Status { get; set; }

    /// <summary>
    /// A sidecar that is terminated by the user or the system can only be started by start
    /// </summary>
    private bool _isStopByManually;

    private System.Timers.Timer? _heartBeatTimer;
    private Process? _process;
    private int _retryTime;

    public DaprProcess(
        IDaprProcessProvider daprProcessProvider,
        IProcessProvider processProvider,
        IOptionsMonitor<DaprOptions> daprOptions,
        IDaprEnvironmentProvider daprEnvironmentProvider,
        IDaprProvider daprProvide,
        ILogger<DaprProcess>? logger = null)
        : base(daprEnvironmentProvider, daprProvide, daprOptions)
    {
        _daprProcessProvider = daprProcessProvider;
        _processProvider = processProvider;
        _logger = logger;
        daprOptions.OnChange(Refresh);
    }

    public void Start()
    {
        if (Status == DaprProcessStatus.Started)
        {
            _logger?.LogInformation("The sidecar has been successfully started. If you want to restart, please stop and start again");
            return;
        }

        lock (_lock)
        {
            _isStopByManually = false;
            var sidecarOptions = ConvertToSidecarOptions(_daprOptions.CurrentValue);

            StartCore(sidecarOptions);
        }
    }

    private void StartCore(SidecarOptions options)
    {
        UpdateStatus(DaprProcessStatus.Starting);
        var commandLineBuilder = CreateCommandLineBuilder(options);

        _process = _daprProcessProvider.DaprStart(
            _defaultSidecarFileName!,
            commandLineBuilder.ToString(),
            options.CreateNoWindow,
            (_, args) =>
            {
                if (args.Data == null)
                    return;

                if (_isFirst) CheckAndCompleteDaprEnvironment(args.Data);
            }, () => UpdateStatus(DaprProcessStatus.Stopped));

        _retryTime = 0;
        if (_heartBeatTimer == null && options.EnableHeartBeat)
        {
            _heartBeatTimer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = options.HeartBeatInterval
            };
            _heartBeatTimer.Elapsed += (_, _) => HeartBeat();
            _heartBeatTimer.Start();
        }
        else
        {
            _heartBeatTimer?.Start();
        }

        // Register the child process to the job object to ensure that the child process terminates when the parent process terminates
        // Windows only

        ChildProcessTracker.AddProcess(_process);
    }

    private void CheckAndCompleteDaprEnvironment(string data)
    {
        var httpPort = GetHttpPort(data);
        var grpcPort = GetGrpcPort(data);

        SuccessDaprOptions!.TrySetHttpPort(httpPort);
        SuccessDaprOptions!.TrySetGrpcPort(grpcPort);
        CompleteDaprEnvironment();
    }

    private void CompleteDaprEnvironment()
    {
        if (SuccessDaprOptions!.DaprHttpPort is > 0 && SuccessDaprOptions!.DaprGrpcPort is > 0)
        {
            DaprEnvironmentProvider.TrySetHttpPort(SuccessDaprOptions.DaprHttpPort);
            DaprEnvironmentProvider.TrySetGrpcPort(SuccessDaprOptions.DaprGrpcPort);
            _isFirst = false;
            _retryTime = 0;
            UpdateStatus(DaprProcessStatus.Started);
        }
    }

    /// <summary>
    /// Only stop sidecars started by the current program
    /// </summary>
    public void Stop()
    {
        lock (_lock)
        {
            switch (Status)
            {
                case DaprProcessStatus.Stopped:
                    _logger?.LogDebug("dapr sidecar stopped, do not repeat stop");
                    return;
                case DaprProcessStatus.Stopping:
                    _logger?.LogDebug("dapr sidecar is stopping, do not repeat, please wait...");
                    return;
                default:
                    if (SuccessDaprOptions == null)
                    {
                        _logger?.LogDebug("There is no dapr sidecar successfully launched by the current program");
                        return;
                    }

                    UpdateStatus(DaprProcessStatus.Stopping);
                    StopCore();
                    _heartBeatTimer?.Stop();
                    _isStopByManually = true;
                    return;
            }
        }
    }

    private void StopCore()
    {
        // In https mode, the dapr process cannot be stopped by dapr stop
        _process?.Kill();
        if (SuccessDaprOptions!.EnableSsl is not true)
        {
            _daprProcessProvider.DaprStop(_defaultSidecarFileName!, SuccessDaprOptions.AppId);
        }

        if (SuccessDaprOptions.DaprHttpPort != null)
            CheckPortAndKill(SuccessDaprOptions.DaprHttpPort.Value);
        if (SuccessDaprOptions.DaprGrpcPort != null)
            CheckPortAndKill(SuccessDaprOptions.DaprGrpcPort.Value);

        UpdateStatus(DaprProcessStatus.Stopped);
    }

    /// <summary>
    /// Refresh the dapr configuration, the source dapr process will be killed and the new dapr process will be restarted
    /// </summary>
    /// <param name="options"></param>
    private void Refresh(DaprOptions options)
    {
        lock (_lock)
        {
            if (_isStopByManually)
            {
                _logger?.LogDebug("configuration update, you need to start dapr through Start (Restart is not supported due to manual stop of sidecar)");
                return;
            }

            var sidecarOptionsByRefresh = ConvertToSidecarOptions(options);
            if (SuccessDaprOptions != null)
            {
                sidecarOptionsByRefresh.AppPort ??= SuccessDaprOptions.AppPort;
                sidecarOptionsByRefresh.EnableSsl ??= SuccessDaprOptions.EnableSsl;
                sidecarOptionsByRefresh.DaprHttpPort ??= SuccessDaprOptions.DaprHttpPort;
                sidecarOptionsByRefresh.DaprGrpcPort ??= SuccessDaprOptions.DaprGrpcPort;

                if (sidecarOptionsByRefresh.Equals(SuccessDaprOptions))
                {
                    return;
                }

                UpdateStatus(DaprProcessStatus.Restarting);
                StopCore();
            }

            _isFirst = true;
            SuccessDaprOptions = null;
            _logger?.LogDebug("Dapr sidecar configuration updated, Dapr AppId is {AppId}, restarting dapr, please wait...", options.AppId);
            StartCore(sidecarOptionsByRefresh);
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
                    DaprStarterConstant.DEFAULT_PROCESS_NAME);
                process.Kill();
            }
        }
    }

    private void HeartBeat()
    {
        lock (_lock)
        {
            if (SuccessDaprOptions == null)
                return;

            if (SuccessDaprOptions!.EnableSsl is true)
            {
                _logger?.LogDebug("The dapr status cannot be monitored in https mode, the check has been skipped");
                return;
            }

            var daprList = _daprProcessProvider.GetDaprList(_defaultDaprFileName!, SuccessDaprOptions.AppId, out bool isException);
            if (daprList.Count > 1)
            {
                _logger?.LogDebug("dapr sidecar appears more than 1 same appid, this may cause error");
            }

            if (!daprList.Any())
            {
                if(isException)
                    return;

                switch (Status)
                {
                    case DaprProcessStatus.Started:
                        _logger?.LogWarning("Dapr sidecar terminated abnormally, restarting, please wait...");
                        StartCore(SuccessDaprOptions);
                        break;
                    case DaprProcessStatus.Starting when _retryTime < DaprStarterConstant.DEFAULT_RETRY_TIME:
                        _retryTime++;
                        _logger?.LogDebug("Dapr is not started: The {Retries}th heartbeat check. AppId is {AppId}",
                            _retryTime,
                            SuccessDaprOptions.AppId);
                        break;
                    case DaprProcessStatus.Starting:
                        _logger?.LogWarning(
                            "Dapr is not started: The {Retries}th heartbeat check. Dapr stopped, restarting, please wait...",
                            _retryTime + 1);
                        StartCore(SuccessDaprOptions);
                        break;
                    case DaprProcessStatus.Restarting:
                        _logger?.LogWarning("Dapr is restarting, the current state is {State}, please wait...", Status);
                        break;
                }
            }
            else
            {
                if (Status == DaprProcessStatus.Starting)
                {
                    // Execute only when getting HttpPort, gRPCPort exception
                    var daprSidecar = daprList.First();
                    SuccessDaprOptions.TrySetHttpPort(daprSidecar.HttpPort);
                    SuccessDaprOptions.TrySetGrpcPort(daprSidecar.GrpcPort);
                    UpdateStatus(DaprProcessStatus.Started);
                }
            }
        }
    }

    private void UpdateStatus(DaprProcessStatus status)
    {
        if (status != Status)
        {
            _logger?.LogDebug("Dapr Process Status Change: {PreStatus} -> {PostStatus}", Status, status);
            Status = status;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Stop();
    }
}
