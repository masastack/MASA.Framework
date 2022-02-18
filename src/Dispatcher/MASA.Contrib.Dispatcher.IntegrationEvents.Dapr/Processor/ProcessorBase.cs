namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public abstract class ProcessorBase : IProcessor
{
    public abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public Task SleepAsync()
    {
        Thread.Sleep(SleepTime);
        return Task.CompletedTask;
    }

    public abstract int SleepTime { get; }
}
