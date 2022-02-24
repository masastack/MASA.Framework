namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IProcessor
{
    Task ExecuteAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Easy to switch between background tasks
    /// </summary>
    /// <param name="delay">unit: seconds</param>
    /// <returns></returns>
    Task DelayAsync(int delay);
}
