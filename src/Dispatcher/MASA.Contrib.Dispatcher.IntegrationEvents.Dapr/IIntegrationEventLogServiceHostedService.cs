namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IIntegrationEventLogServiceHostedService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
