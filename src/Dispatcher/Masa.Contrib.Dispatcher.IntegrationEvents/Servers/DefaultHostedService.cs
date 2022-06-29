// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Servers;

public class DefaultHostedService : IProcessingServer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IProcessor> _processors;

    public DefaultHostedService(IServiceProvider serviceProvider, IEnumerable<IProcessor> processors)
    {
        _serviceProvider = serviceProvider;
        _processors = processors;
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_serviceProvider.GetService<IUnitOfWorkManager>() == null)
            return Task.CompletedTask;

        var processorTasks = _processors.Select(processor => new InfiniteLoopProcessor(_serviceProvider, processor))
            .Select(process => process.ExecuteAsync(stoppingToken));
        return Task.WhenAll(processorTasks);
    }
}
