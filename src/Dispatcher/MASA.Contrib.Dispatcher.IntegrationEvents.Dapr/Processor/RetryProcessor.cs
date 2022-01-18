namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryProcessor : IProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RetryProcessor> _logger;
    private readonly DaprClient _dapr;
    private readonly DispatcherOptions _options;
    private readonly IOptionsMonitor<AppConfig> _appConfig;
    private readonly StateOptions _stateOptions;
    private readonly string _defaultTtLttlInSeconds;
    private readonly string _storeName;

    public RetryProcessor(
        IServiceProvider serviceProvider,
        ILogger<RetryProcessor> logger,
        DaprClient dapr,
        IOptionsMonitor<AppConfig> appConfig,
        IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _dapr = dapr;
        _appConfig = appConfig;
        _options = options.Value;
        _stateOptions = new StateOptions()
        {
            Consistency = ConsistencyMode.Eventual,
            Concurrency = ConcurrencyMode.FirstWrite
        };
        _defaultTtLttlInSeconds = "3";
        _storeName = "statestore";
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
                unitOfWork.UseTransaction = false;

            var eventLogService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();

            var retrieveEventLogs =
                await eventLogService
                    .RetrieveEventLogsPendingToPublishAsync(Guid
                        .Empty); //todo: Need to be replaced with RetrieveEventLogsFailedToPublishAsync method

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    var stateAndETag = await
                        _dapr.GetStateAndETagAsync<Guid>(_storeName, eventLog.Id.ToString(), ConsistencyMode.Eventual,
                            cancellationToken: stoppingToken);

                    if (stateAndETag.value != Guid.Empty)
                        continue;

                    bool isSave = await _dapr.TrySaveStateAsync(
                        _storeName,
                        eventLog.Id.ToString(),
                        Guid.NewGuid(),
                        stateAndETag.etag, _stateOptions, new Dictionary<string, string>()
                        {
                            {"TTLttlInSeconds", _defaultTtLttlInSeconds}
                        }, stoppingToken);
                    if (!isSave)
                        continue;

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger.LogInformation("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                        _options.PubSubName,
                        eventLog.Event.Topic);
                    await _dapr.PublishEventAsync(_options.PubSubName, eventLog.Event.Topic, (dynamic) eventLog);

                    await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
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

        await Task.Delay(_options.FailedRetryInterval, stoppingToken);
    }
}
