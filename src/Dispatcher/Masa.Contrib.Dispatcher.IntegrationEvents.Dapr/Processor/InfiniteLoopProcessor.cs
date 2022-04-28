// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class InfiniteLoopProcessor : ProcessorBase
{
    private readonly IProcessor _processor;
    private readonly ILogger<InfiniteLoopProcessor>? _logger;

    public InfiniteLoopProcessor(IServiceProvider serviceProvider, IProcessor processor)
        : base(serviceProvider)
    {
        _processor = processor;
        _logger = serviceProvider.GetService<ILogger<InfiniteLoopProcessor>>();
    }

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _processor.ExecuteAsync(stoppingToken);
                await DelayAsync(((ProcessorBase)_processor).Delay);
            }
            catch (OperationCanceledException ex)
            {
                //ignore
                _logger?.LogWarning("Operation canceled", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Processor '{ProcessorName}' failed", _processor.ToString());

                await DelayAsync(2);
            }
        }
    }
}
