namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IProcessor
{
    Task ExecuteAsync(CancellationToken stoppingToken);

    Task DelayAsync(int delay);
}
