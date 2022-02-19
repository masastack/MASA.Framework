namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryByDataProcessor : ProcessorBase, IProcessor
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

    public override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
                unitOfWork.UseTransaction = false;

            var dapr = _serviceProvider.GetRequiredService<DaprClient>();
            var eventLogService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();

            var retrieveEventLogs =
                await eventLogService.RetrieveEventLogsFailedToPublishAsync(_options.Value.RetryBatchSize, _options.Value.MaxRetryTimes, _options.Value.MinimumRetryInterval);

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    if (LocalQueueProcessor.Default.IsExist(eventLog.EventId))
                        continue; // The local queue is retrying, no need to retry

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger.LogDebug("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                        _options.Value.PubSubName,
                        eventLog.Event.Topic);

                    await dapr.PublishEventAsync(_options.Value.PubSubName, eventLog.Event.Topic, eventLog.Event, stoppingToken);

                    LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);

                    await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
                }
                catch (UserFriendlyException)
                {
                    //Update state due to multitasking contention, no processing required
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
    }

    public override int SleepTime => _options.Value.FailedRetryInterval;
}
