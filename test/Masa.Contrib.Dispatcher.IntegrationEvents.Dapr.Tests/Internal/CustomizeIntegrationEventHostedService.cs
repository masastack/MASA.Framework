namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Internal;

public class CustomizeIntegrationEventHostedService : IntegrationEventHostedService
{
    public CustomizeIntegrationEventHostedService(
        IProcessingServer processingServer, ILoggerFactory? loggerFactory = null) :
        base(processingServer, loggerFactory?.CreateLogger<IntegrationEventHostedService>())
    {
    }

    public Task TestExecuteAsync(CancellationToken stoppingToken)
    {
        return ExecuteAsync(stoppingToken);
    }
}
