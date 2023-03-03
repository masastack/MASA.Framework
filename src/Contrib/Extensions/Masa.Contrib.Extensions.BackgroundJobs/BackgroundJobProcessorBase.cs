// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public abstract class BackgroundJobProcessorBase : IBackgroundJobProcessor
{
    protected readonly IServiceProvider ServiceProvider;
    private readonly Timer _timer;
    private volatile bool _isRunning;
    private readonly BackgroundJobRelationNetwork _relationNetwork;
    private CancellationToken _cancellationToken;
    protected readonly ILogger<BackgroundJobProcessorBase>? Logger;
    protected readonly IDeserializer Deserializer;

    /// <summary>
    /// Background task status
    /// default task: terminated
    /// </summary>
    private bool _isTerminated = true;

    public virtual int Period => 5000;

    public BackgroundJobProcessorBase(IServiceProvider serviceProvider, IDeserializer deserializer)
    {
        ServiceProvider = serviceProvider;
        _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        _relationNetwork = ServiceProvider.GetRequiredService<BackgroundJobRelationNetwork>();
        Logger = ServiceProvider.GetService<ILogger<BackgroundJobProcessorBase>>();
        Deserializer = deserializer;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        MasaArgumentException.ThrowIfLessThanOrEqual(Period, 0);

        lock (_timer)
        {
            _cancellationToken = cancellationToken;
            _isRunning = true;
            ChangeTimer(Period, Timeout.Infinite);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        lock (_timer)
        {
            _cancellationToken = cancellationToken;
            _isRunning = false;
            ChangeTimer(Timeout.Infinite, Timeout.Infinite);
        }
        return Task.CompletedTask;
    }

    private void TimerCallback(object? state)
    {
        lock (_timer)
        {
            if (!_isRunning || !_isTerminated) return;

            ChangeTimer(Timeout.Infinite, Timeout.Infinite);
            _isTerminated = false;
        }

        if (_cancellationToken.IsCancellationRequested)
        {
            Logger?.LogDebug("----- Background task was terminated");
            return;
        }

        _ = ExecuteJobAsync();
    }

    private async Task ExecuteJobAsync()
    {
        try
        {
            await ExecuteJobAsync(_cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "----- Error executing background task");
        }
        finally
        {
            lock (_timer)
            {
                _isTerminated = true;
                ChangeTimer(Period, Timeout.Infinite);
            }
        }
    }

    protected abstract Task ExecuteJobAsync(CancellationToken cancellationToken);

    protected List<Type> GetJobTypeList(string jobName)
        => _relationNetwork.GetJobTypeList(jobName);

    protected Type GetJobArgsType(string jobName)
        => _relationNetwork.GetJobArgsType(jobName);

    protected object? GetJobArgs(string jobArgs, Type jobArgsType)
        => Deserializer.Deserialize(jobArgs, jobArgsType);

    private void ChangeTimer(int dueTime, int period)
        => _timer.Change(dueTime != Timeout.Infinite ? dueTime * 1000 : dueTime, period);
}
