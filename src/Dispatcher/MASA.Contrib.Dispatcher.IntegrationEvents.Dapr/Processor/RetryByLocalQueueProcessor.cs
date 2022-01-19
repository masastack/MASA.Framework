namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryByLocalQueueProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<AppConfig> _appConfig;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<RetryByLocalQueueProcessor> _logger;

    public RetryByLocalQueueProcessor(
        IServiceProvider serviceProvider,
        IOptionsMonitor<AppConfig> appConfig,
        IOptions<DispatcherOptions> options,
        ILogger<RetryByLocalQueueProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _appConfig = appConfig;
        _options = options;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
                unitOfWork.UseTransaction = false;

            var dapr = _serviceProvider.GetRequiredService<DaprClient>();
            var eventLogService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();

            var retrieveEventLogs = LocalQueueProcessor.Default.RetrieveEventLogsFailedToPublishAsync();

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger.LogInformation("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                        _options.Value.PubSubName,
                        eventLog.Topic);

                    await dapr.PublishEventAsync(_options.Value.PubSubName, eventLog.Topic, eventLog.Event, stoppingToken);

                    await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);

                    LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                        eventLog.EventId, _appConfig.CurrentValue.AppId, eventLog);
                    await eventLogService.MarkEventAsFailedAsync(eventLog.EventId);
                }
                finally
                {
                    if (unitOfWork != null && unitOfWork.TransactionHasBegun)
                        await unitOfWork.CommitAsync(stoppingToken);
                }
            }
        }

        await Task.Delay(_options.Value.LocalFailedRetryInterval, stoppingToken);
    }
}
