namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public abstract class ProcessorBase : IProcessor
{
    public abstract Task ExecuteAsync(CancellationToken stoppingToken);

    // /// <summary>
    // /// Easy to switch between background tasks
    // /// </summary>
    /// <param name="delay">unit: seconds</param>
    // /// <returns></returns>
    public Task DelayAsync(int delay)
        => Task.Delay(TimeSpan.FromSeconds(delay));

    /// <summary>
    /// Task delay time, unit: seconds
    /// </summary>
    public virtual int Delay { get; }
}
