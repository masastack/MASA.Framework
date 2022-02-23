namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Servers;

public class DefaultHostedService : IProcessingServer
{
    private readonly IEnumerable<IProcessor> _processors;
    private readonly ILogger<InfiniteLoopProcessor>? _logger;

    public DefaultHostedService(IEnumerable<IProcessor> processors, ILogger<InfiniteLoopProcessor>? logger = null)
    {
        _processors = processors;
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processorTasks = _processors.Select(processor => new InfiniteLoopProcessor(processor, _logger))
            .Select(process => process.ExecuteAsync(stoppingToken));
        return Task.WhenAll(processorTasks);
    }
}
