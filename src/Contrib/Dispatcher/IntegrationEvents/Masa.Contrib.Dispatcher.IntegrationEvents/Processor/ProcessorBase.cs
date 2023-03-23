// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public abstract class ProcessorBase : IProcessor
{
    protected readonly IServiceProvider? ServiceProvider;

    /// <summary>
    /// Task delay time, unit: seconds
    /// </summary>
    public virtual int Delay { get; }

    protected ProcessorBase(IServiceProvider? serviceProvider) => ServiceProvider = serviceProvider;

    public virtual async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (ServiceProvider != null)
        {
            var unitOfWorkManager = ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            var localMessageDbConnectionString = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ILocalMessageDbConnectionStringProvider>();
            var connectionStrings = localMessageDbConnectionString.ConnectionStrings;
            foreach (var connectionString in connectionStrings)
            {
                await using var unitOfWork = unitOfWorkManager.CreateDbContext(new DbContextConnectionStringOptions(connectionString));
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
