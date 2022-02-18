namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IProcessingServer
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
