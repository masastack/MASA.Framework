// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr.AspNetCore;

public class DaprBackgroundService : BackgroundService
{
    private readonly IAppPortProvider _appPortProvider;
    private readonly IDaprProcess _daprProcess;
    private readonly DaprOptions _options;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<DaprBackgroundService>? _logger;

    public DaprBackgroundService(
        IAppPortProvider appPortProvider,
        IDaprProcess daprProcess,
        IOptionsMonitor<DaprOptions> options,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<DaprBackgroundService>? logger)
    {
        _appPortProvider = appPortProvider;
        _daprProcess = daprProcess;
        _options = options.CurrentValue;
        options.OnChange(daprOptions =>
        {
            CheckCompletionAppPort(daprOptions);
            _daprProcess.Refresh(daprOptions);
        });
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForAppStartup(_hostApplicationLifetime, stoppingToken))
            return;

        // if cancellation was requested, stop
        if (stoppingToken.IsCancellationRequested)
        {
            _logger?.LogInformation("{Name} is Stopping...", nameof(DaprBackgroundService));
            _daprProcess.Stop(stoppingToken);
        }
        else
        {
            _logger?.LogInformation("{Name} is Starting ...", nameof(DaprBackgroundService));

            CheckCompletionAppPort(_options);

            _daprProcess.Start(_options, stoppingToken);
        }
    }

    private void CheckCompletionAppPort(DaprOptions daprOptions)
    {
        if (daprOptions.AppPort == null)
        {
            CompletionAppPort(daprOptions);
        }
    }

    private void CompletionAppPort(DaprOptions daprOptions)
    {
        var item = _appPortProvider.GetAppPort(daprOptions.EnableSsl);
        if (daprOptions.EnableSsl == null)
        {
            daprOptions.EnableSsl = item.EnableSsl;
            daprOptions.AppPort = item.AppPort;
        }
        else if (daprOptions.EnableSsl == item.EnableSsl)
        {
            daprOptions.AppPort = item.AppPort;
        }
        else
        {
            throw new UserFriendlyException(
                $"The current project does not support {(daprOptions.EnableSsl is true ? "Https" : "Http")}, Dapr failed to start");
        }
    }

    static async Task<bool> WaitForAppStartup(IHostApplicationLifetime hostApplicationLifetime, CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();

        await using var startedCancellationTokenRegistration =
            hostApplicationLifetime.ApplicationStarted.Register(() => startedSource.SetResult());
        await using var cancellationTokenRegistration = stoppingToken.Register(() => cancelledSource.SetResult());

        Task completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);

        // If the completed tasks was the "app started" task, return true, otherwise false
        return completedTask == startedSource.Task;
    }
}
