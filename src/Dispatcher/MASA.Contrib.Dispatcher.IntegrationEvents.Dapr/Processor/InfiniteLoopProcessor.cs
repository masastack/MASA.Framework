namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class InfiniteLoopProcessor : IProcessor
{
    private readonly IProcessor _processor;
    private readonly Logger<InfiniteLoopProcessor> _logger;

    public InfiniteLoopProcessor(IProcessor processor,Logger<InfiniteLoopProcessor> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _processor.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                //ignore
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Processor '{ProcessorName}' failed", _processor.ToString());

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
