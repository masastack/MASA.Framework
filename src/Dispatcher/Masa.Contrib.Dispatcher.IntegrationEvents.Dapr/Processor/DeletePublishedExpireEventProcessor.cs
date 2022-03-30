namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeletePublishedExpireEventProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;

    public override int Delay => _options.Value.CleaningExpireInterval;

    public DeletePublishedExpireEventProcessor(IServiceProvider serviceProvider, IOptions<DispatcherOptions> options)
        : base(serviceProvider)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="stoppingToken"></param>
    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        var expireDate = (_options.Value.GetCurrentTime?.Invoke() ?? DateTime.UtcNow).AddSeconds(-_options.Value.PublishedExpireTime);
        await logService.DeleteExpiresAsync(expireDate, _options.Value.DeleteBatchCount, stoppingToken);
    }
}
