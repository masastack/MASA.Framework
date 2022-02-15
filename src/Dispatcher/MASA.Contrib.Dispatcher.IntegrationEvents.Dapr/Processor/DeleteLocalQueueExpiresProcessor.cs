namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeleteLocalQueueExpiresProcessor : IProcessor
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
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LocalQueueProcessor.Default.DeleteAsync(_options.Value.LocalRetryTimes);
        await Task.Delay(_options.Value.CleaningLocalQueueExpireInterval, stoppingToken);
    }
}
