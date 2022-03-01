namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Internal;

public class IntegrationEventHostedService : Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.IntegrationEventHostedService
{
    public IntegrationEventHostedService(
        IProcessingServer processingServer, ILogger<Dapr.IntegrationEventHostedService>? logger = null) :
        base(processingServer, logger)
    {
    }

    public Task TestExecuteAsync(CancellationToken stoppingToken)
    {
        return ExecuteAsync(stoppingToken);
    }
}
