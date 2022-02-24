namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class IntegrationEventHostedService : BackgroundService
{
    private readonly ILogger<IntegrationEventHostedService>? _logger;
    private readonly IProcessingServer _processingServer;

    public IntegrationEventHostedService(IProcessingServer processingServer, ILogger<IntegrationEventHostedService>? logger)
    {
        _logger = logger;
        _processingServer = processingServer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger?.LogDebug("----- IntegrationEvent background task is starting");

        return _processingServer.ExecuteAsync(stoppingToken);
    }
}
