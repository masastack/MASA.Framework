namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class DeletePublishedExpireEventProcessor : ProcessorBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<DispatcherOptions> _options;

    public DeletePublishedExpireEventProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var logService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var expireDate = (_options.Value.GetCurrentTime?.Invoke() ?? DateTime.UtcNow).AddSeconds(-_options.Value.PublishedExpireTime);
            await logService.DeleteExpiresAsync(expireDate, _options.Value.DeleteBatchCount, stoppingToken);
        }
    }

    public override int Delay => _options.Value.CleaningExpireInterval * 1000;
}
