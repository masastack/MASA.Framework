// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarter;

[ExcludeFromCodeCoverage]
public class DaprProcess : DaprProcessBase, IDaprProcess
{
    private readonly object _lock = new();

    private readonly IDaprProvider _daprProvider;
    private readonly IProcessProvider _processProvider;
    private readonly ILogger<DaprProcess>? _logger;
    private readonly IOptionsMonitor<DaprOptions> _daprOptions;
    private DaprProcessStatus Status { get; set; }
    private System.Timers.Timer? _heartBeatTimer;
    private Process? _process;
    private int _retryTime;

    /// <summary>
    /// record whether dapr is initialized for the first time
    /// </summary>
    private bool _isFirst = true;

    public DaprProcess(
        IDaprProvider daprProvider,
        IProcessProvider processProvider,
        IOptionsMonitor<DaprOptions> daprOptions,
        IDaprEnvironmentProvider daprEnvironmentProvider,
        ILogger<DaprProcess>? logger = null,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null) : base(daprEnvironmentProvider, masaAppConfigureOptions)
    {
        _daprProvider = daprProvider;
        _processProvider = processProvider;
        _logger = logger;
        _daprOptions = daprOptions;
        daprOptions.OnChange(Refresh);
    }

    public void Start()
    {
        lock (_lock)
        {
            var options = ConvertToDaprCoreOptions(_daprOptions.CurrentValue);

            StartCore(options);
        }
    }

    private void StartCore(DaprCoreOptions options)
    {
        UpdateStatus(DaprProcessStatus.Starting);
        var commandLineBuilder = CreateCommandLineBuilder(options);
        StopCore(SuccessDaprOptions);

        if (_isFirst)
        {
            CompleteDaprEnvironment(options.DaprHttpPort, options.DaprGrpcPort);
        }

        _process = _daprProvider.DaprStart(
            commandLineBuilder.ToString(),
            options.CreateNoWindow,
            (_, args) =>
            {
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
    }

    public void CompleteDaprEnvironment(ushort? httpPort, ushort? grpcPort)
    {
        var setHttpPortResult = DaprEnvironmentProvider.TrySetHttpPort(httpPort);
        if (setHttpPortResult)
        {
            SuccessDaprOptions!.TrySetHttpPort(httpPort);
            _logger?.LogInformation("Update Dapr environment variables, DaprHttpPort: {HttpPort}", httpPort);
        }

        var setGrpcPortResult = DaprEnvironmentProvider.TrySetGrpcPort(grpcPort);
        if (setGrpcPortResult)
        {
            SuccessDaprOptions!.TrySetGrpcPort(grpcPort);
            _logger?.LogInformation("Update Dapr environment variables, DAPR_GRPC_PORT: {grpcPort}", grpcPort);
        }

        if (setHttpPortResult && setGrpcPortResult) _isFirst = false;
    }

    public void CheckAndCompleteDaprEnvironment(string? data)
    {
        if (data == null)
            return;

        var httpPort = GetHttpPort(data);
        var grpcPort = GetgRPCPort(data);

        CompleteDaprEnvironment(httpPort, grpcPort);
    }

    public void Stop()
    {
        lock (_lock)
        {
            StopCore(SuccessDaprOptions);
            _heartBeatTimer?.Stop();
        }
    }

    private void StopCore(DaprCoreOptions? options)
    {
        if (options != null)
        {
            // In https mode, the dapr process cannot be stopped by dapr stop
            if (options.EnableSsl is true)
            {
                _process?.Kill();
            }
            else
            {
                _daprProvider.DaprStop(options.AppId);
            }

            if (options.DaprHttpPort != null)
                CheckPortAndKill(options.DaprHttpPort.Value);
            if (options.DaprGrpcPort != null)
                CheckPortAndKill(options.DaprGrpcPort.Value);
        }
    }

    /// <summary>
    /// Refresh the dapr configuration, the source dapr process will be killed and the new dapr process will be restarted
    /// </summary>
    /// <param name="options"></param>
    public void Refresh(DaprOptions options)
    {
        lock (_lock)
        {
            _logger?.LogDebug("Dapr configuration refresh, Dapr AppId is {AppId}, please wait...", SuccessDaprOptions!.AppId);

            if (SuccessDaprOptions != null)
            {
                options.AppPort = SuccessDaprOptions.AppPort;
                options.EnableSsl = SuccessDaprOptions.EnableSsl;
                options.DaprHttpPort = SuccessDaprOptions.DaprHttpPort;
                options.DaprGrpcPort = SuccessDaprOptions.DaprGrpcPort;

                UpdateStatus(DaprProcessStatus.Restarting);
                _logger?.LogDebug(
                    "Dapr configuration refresh, Dapr AppId is {AppId}, closing dapr, please wait...",
                    SuccessDaprOptions!.AppId);
                StopCore(SuccessDaprOptions);
            }

            _isFirst = true;
            SuccessDaprOptions = null;
            _logger?.LogDebug("Dapr configuration refresh, Dapr AppId is {AppId}, restarting dapr, please wait...", options.AppId);
            StartCore(ConvertToDaprCoreOptions(options));
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
                    Constant.DEFAULT_FILE_NAME);
                process.Kill();
            }
        }
    }

    private void HeartBeat()
    {
        lock (_lock)
        {
            if (SuccessDaprOptions!.EnableSsl is true)
            {
                _logger?.LogDebug("The dapr status cannot be monitored in https mode, the check has been skipped");
                return;
            }

            if (!_daprProvider.IsExist(SuccessDaprOptions!.AppId))
            {
                if (Status == DaprProcessStatus.Started || Status == DaprProcessStatus.Stopped)
                {
                    _logger?.LogWarning("Dapr stopped, restarting, please wait...");
                    StartCore(SuccessDaprOptions);
                }
                else if (Status == DaprProcessStatus.Starting)
                {
                    if (_retryTime < Constant.DEFAULT_RETRY_TIME)
                    {
                        _retryTime++;
                        _logger?.LogDebug("Dapr is not started: The {Retries}th heartbeat check. AppId is {AppId}",
                            _retryTime,
                            SuccessDaprOptions.AppId);
                    }
                    else
                    {
                        _logger?.LogWarning(
                            "Dapr is not started: The {Retries}th heartbeat check. Dapr stopped, restarting, please wait...",
                            _retryTime + 1);
                        StartCore(SuccessDaprOptions);
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
