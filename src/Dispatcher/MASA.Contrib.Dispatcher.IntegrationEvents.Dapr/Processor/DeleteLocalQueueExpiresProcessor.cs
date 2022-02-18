namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteLocalQueueExpiresProcessor : ProcessorBase, IProcessor
{
    private readonly IOptions<DispatcherOptions> _options;

    public DeleteLocalQueueExpiresProcessor(IOptions<DispatcherOptions> options)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LocalQueueProcessor.Default.Delete(_options.Value.LocalRetryTimes);
        return Task.CompletedTask;
    }

    public override int SleepTime => _options.Value.CleaningLocalQueueExpireInterval;
}
