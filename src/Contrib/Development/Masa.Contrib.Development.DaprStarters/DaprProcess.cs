// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters;

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
        ILogger<DaprProcess>? logger = null,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null) : base(masaAppConfigureOptions)
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
            var options = ConvertTo(_daprOptions.CurrentValue);

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
    }

    public void CompleteDaprEnvironment(ushort? httpPort, ushort? grpcPort)
    {
        if (grpcPort is > 0)
            CompleteDaprGrpcPortEnvironment(grpcPort.Value);

        if (httpPort is > 0)
            CompleteDaprHttpPortEnvironment(httpPort.Value);

        if (httpPort is > 0 && grpcPort is > 0)
        {
            SuccessDaprOptions!.SetPort(httpPort.Value, grpcPort.Value);
            CompleteDaprEnvironment(httpPort.Value, grpcPort.Value, () => _isFirst = false);
        }
    }

    public void CheckAndCompleteDaprEnvironment(string? data)
    {
        if (data == null)
            return;

        var httpPort = GetHttpPort(data);
        var grpcPort = GetGrpcPort(data);

        CompleteDaprEnvironment(httpPort, grpcPort);
    }

    /// <summary>
    /// Improve the information of HttpPort and GrpcPort successfully configured.
    /// When Port is specified or Dapr is closed for other reasons after startup, the HttpPort and GrpcPort are the same as the Port assigned at the first startup.
    /// </summary>
    private void CompleteDaprEnvironment(ushort httpPort, ushort grpcPort, Action action)
    {
        if (CompleteDaprHttpPortEnvironment(httpPort) & CompleteDaprGrpcPortEnvironment(grpcPort))
        {
            action.Invoke();
            _logger?.LogInformation(
                "Update Dapr environment variables, DaprHttpPort: {HttpPort}, DAPR_GRPC_PORT: {GrpcPort}",
                httpPort,
                grpcPort);
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            StopCore(SuccessDaprOptions);
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
    /// todo: At present, there are no restrictions on HttpPort and GrpcPort, but if the configuration update changes HttpPort and GrpcPort, the port obtained by DaprClient will be inconsistent with the actual operation, which needs to be adjusted later.
    /// </summary>
    /// <param name="options"></param>
    public void Refresh(DaprOptions options)
    {
        lock (_lock)
        {
            _logger?.LogDebug("Dapr configuration refresh, Dapr AppId is {AppId}, please wait...", SuccessDaprOptions!.AppId);

            if (SuccessDaprOptions != null)
            {
                UpdateStatus(DaprProcessStatus.Restarting);
                _logger?.LogDebug(
                    "Dapr configuration refresh, Dapr AppId is {AppId}, closing dapr, please wait...",
                    SuccessDaprOptions!.AppId);
                StopCore(SuccessDaprOptions);
            }

            _isFirst = true;
            SuccessDaprOptions = null;
            _logger?.LogDebug("Dapr configuration refresh, Dapr AppId is {AppId}, restarting dapr, please wait...", options.AppId);
            StartCore(ConvertTo(options));
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

    public void Dispose() => Stop();
}
