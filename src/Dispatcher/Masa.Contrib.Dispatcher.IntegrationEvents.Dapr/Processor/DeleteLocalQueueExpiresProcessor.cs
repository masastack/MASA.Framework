namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteLocalQueueExpiresProcessor : ProcessorBase
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

    public override int Delay => _options.Value.CleaningLocalQueueExpireInterval;
}
