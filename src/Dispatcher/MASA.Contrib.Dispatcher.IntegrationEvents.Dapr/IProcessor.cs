namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public interface IProcessor
{
    Task ExecuteAsync(CancellationToken stoppingToken);

    Task SleepAsync();

    /// <summary>
    /// The time to rest after the task is executed, in milliseconds
    /// </summary>
    int SleepTime { get; }
}
