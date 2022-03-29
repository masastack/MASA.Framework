namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteLocalQueueExpiresProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;

    public override int Delay => _options.Value.CleaningLocalQueueExpireInterval;

    public DeleteLocalQueueExpiresProcessor(IOptions<DispatcherOptions> options) : base(null)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override Task ExecutingAsync(CancellationToken stoppingToken)
    {
        LocalQueueProcessor.Default.Delete(_options.Value.LocalRetryTimes);
        return Task.CompletedTask;
    }
}
