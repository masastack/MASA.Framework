namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class InfiniteLoopProcessor : ProcessorBase
{
    private readonly IProcessor _processor;
    private readonly ILogger<InfiniteLoopProcessor>? _logger;

    public InfiniteLoopProcessor(IProcessor processor, ILogger<InfiniteLoopProcessor>? logger = null)
    {
        _processor = processor;
        _logger = logger;
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
