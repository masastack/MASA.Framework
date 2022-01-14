namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class DefaultIntegrationEventLogServiceHostedService : IIntegrationEventLogServiceHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventHostedService> _logger;
    private readonly DaprClient _dapr;
    private readonly IOptionsMonitor<AppConfig> _appConfig;
    private readonly string _daprPubsubName;
    private readonly int _idleTime;
    private readonly int _retryDepth;
    private readonly StateOptions _stateOptions;
    private readonly string _storeName;
    private readonly string _defaultTtLttlInSeconds;
    private readonly bool _isRetry;

    public DefaultIntegrationEventLogServiceHostedService(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventHostedService> logger,
        DaprClient dapr,
        IOptionsMonitor<AppConfig> appConfig,
        IOptions<DispatcherOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _dapr = dapr;
        _appConfig = appConfig;
        _daprPubsubName = options.Value.PubSubName;
        _idleTime = options.Value.IdleTime;
        _retryDepth = options.Value.RetryDepth;
        _stateOptions = new StateOptions()
        {
            Consistency = ConsistencyMode.Eventual,
            Concurrency = ConcurrencyMode.FirstWrite
        };
        _storeName = "statestore";
        _defaultTtLttlInSeconds = "3";
        _isRetry = options.Value.IsRetry;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_isRetry)
            return;

        await Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var eventLogService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
                    var retrieveEventLogs = GetResult(() => eventLogService.RetrieveEventLogsPendingToPublishAsync(Guid.Empty));//todo: Need to be replaced with RetrieveEventLogsFailedToPublishAsync method
                    foreach (var eventLog in retrieveEventLogs)
                    {
                        try
                        {
                            var stateAndETag = GetResult(() =>
                                _dapr.GetStateAndETagAsync<Guid>(_storeName, eventLog.Id.ToString(), ConsistencyMode.Eventual,
                                    cancellationToken: stoppingToken));

                            if (stateAndETag.value != Guid.Empty)
                                continue;

                            bool isSave = GetResult(() => _dapr.TrySaveStateAsync(_storeName, eventLog.Id.ToString(),
                                Guid.NewGuid(),
                                stateAndETag.etag, _stateOptions, new Dictionary<string, string>()
                                {
                                    {"TTLttlInSeconds", _defaultTtLttlInSeconds}
                                }, stoppingToken));
                            if (!isSave)
                                continue;

                            GetResult(() => eventLogService.MarkEventAsInProgressAsync(eventLog.EventId));

                            _logger.LogInformation("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                                _daprPubsubName,
                                eventLog.Event.Topic);
                            GetResult(() => _dapr.PublishEventAsync(_daprPubsubName, eventLog.Event.Topic, (dynamic) eventLog));

                            GetResult(() => eventLogService.MarkEventAsPublishedAsync(eventLog.EventId));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                                eventLog.EventId, _appConfig.CurrentValue.AppId, eventLog);
                            GetResult(() => eventLogService.MarkEventAsFailedAsync(eventLog.EventId));
                        }
                        finally
                        {
                            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                            if (unitOfWork != null && unitOfWork.TransactionHasBegun)
                                unitOfWork.CommitAsync(stoppingToken);
                        }
                    }
                }

                Thread.Sleep(_idleTime * 1000);
            }
        }, stoppingToken);
    }

    private static T GetResult<T>(Func<Task<T>> func) => func.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();

    private static void GetResult(Func<Task> func) => func.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();
}
