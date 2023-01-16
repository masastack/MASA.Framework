// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public abstract class ProcessorBase : IProcessor
{
    private readonly IServiceProvider? _serviceProvider;
    internal readonly LocalQueueEventLogService LocalQueueEventLogService;

    /// <summary>
    /// Task delay time, unit: seconds
    /// </summary>
    public virtual int Delay { get; }

    protected ProcessorBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        LocalQueueEventLogService = serviceProvider.GetRequiredService<LocalQueueEventLogService>();
    }

    public virtual async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_serviceProvider != null)
        {
            var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
            var dataConnectionStringProvider = _serviceProvider.GetRequiredService<IDbConnectionStringProvider>();
            var optionsList = dataConnectionStringProvider.DbContextOptionsList;
            foreach (var option in optionsList)
            {
                await using var unitOfWork = unitOfWorkManager.CreateDbContext(option);
                await ExecuteAsync(unitOfWork.ServiceProvider, stoppingToken);
            }
        }
        else
        {
            Executing();
        }
    }

    // /// <summary>
    // /// Easy to switch between background tasks
    // /// </summary>
    /// <param name="delay">unit: seconds</param>
    // /// <returns></returns>
    public Task DelayAsync(int delay) => Task.Delay(TimeSpan.FromSeconds(delay));

    protected virtual Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken) => Task.CompletedTask;

    protected virtual void Executing() { }
}
