namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryByDataProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RetryByDataProcessor> _logger;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly IOptionsMonitor<AppConfig> _appConfig;

    public RetryByDataProcessor(
        IServiceProvider serviceProvider,
        ILogger<RetryByDataProcessor> logger,
        IOptionsMonitor<AppConfig> appConfig,
        IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _appConfig = appConfig;
        _options = options;
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

            // todo: Need to be replaced with RetrieveEventLogsFailedToPublishAsync method
            var retrieveEventLogs = await eventLogService.RetrieveEventLogsPendingToPublishAsync(Guid.Empty);

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    if (LocalQueueProcessor.Default.IsSkipJobs(eventLog.EventId))
                        continue; // The local queue is retrying, no need to retry

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger.LogInformation("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                        _options.Value.PubSubName,
                        eventLog.Event.Topic);

                    await dapr.PublishEventAsync(_options.Value.PubSubName, eventLog.Event.Topic, eventLog.Event, stoppingToken);

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

        await Task.Delay(_options.Value.FailedRetryInterval, stoppingToken);
    }
}
