namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

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
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Processor '{ProcessorName}' failed", _processor.ToString());

                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}
