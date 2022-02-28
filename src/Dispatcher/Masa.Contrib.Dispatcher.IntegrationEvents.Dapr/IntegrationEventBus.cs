namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class IntegrationEventBus : IIntegrationEventBus
{
    private readonly DispatcherOptions _dispatcherOptions;
    private readonly DaprClient _dapr;
    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly IOptionsMonitor<AppConfig>? _appConfig;
    private readonly string _daprPubsubName;
    private readonly IEventBus? _eventBus;
    private readonly IUnitOfWork? _unitOfWork;

    public IntegrationEventBus(IOptions<DispatcherOptions> options,
        DaprClient dapr,
        IIntegrationEventLogService eventLogService,
        IOptionsMonitor<AppConfig>? appConfig = null,
        ILogger<IntegrationEventBus>? logger = null,
        IEventBus? eventBus = null,
        IUnitOfWork? unitOfWork = null)
    {
        _dispatcherOptions = options.Value;
        _dapr = dapr;
        _eventLogService = eventLogService;
        _appConfig = appConfig;
        _logger = logger;
        _daprPubsubName = options.Value.PubSubName;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<Type> GetAllEventTypes() =>
        _eventBus == null
            ? _dispatcherOptions.AllEventTypes
            : _dispatcherOptions.AllEventTypes.Concat(_eventBus.GetAllEventTypes()).Distinct();

    public async Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await PublishIntegrationAsync(integrationEvent);
        }
        else if (_eventBus != null)
        {
            await _eventBus.PublishAsync(@event);
        }
        else
        {
            throw new NotSupportedException(nameof(@event));
        }
    }

    private async Task PublishIntegrationAsync<TEvent>(TEvent @event)
        where TEvent : IIntegrationEvent
    {
        if (@event.UnitOfWork == null && _unitOfWork != null)
            @event.UnitOfWork = _unitOfWork;

        var topicName = @event.Topic;
        if (@event.UnitOfWork != null && @event.UnitOfWork.UseTransaction)
        {
            try
            {
                _logger?.LogDebug("----- Saving changes and integrationEvent: {IntegrationEventId}", @event.Id);
                await _eventLogService.SaveEventAsync(@event, @event.UnitOfWork!.Transaction);

                _logger?.LogDebug(
                    "----- Publishing integration event: {IntegrationEventIdPublished} from {AppId} - ({IntegrationEvent})", @event.Id,
                    _appConfig?.CurrentValue.AppId ?? string.Empty, @event);

                await _eventLogService.MarkEventAsInProgressAsync(@event.Id);

                _logger?.LogDebug("Publishing event {Event} to {PubsubName}.{TopicName}", @event, _daprPubsubName, topicName);
                await _dapr.PublishEventAsync(_daprPubsubName, topicName, (dynamic)@event);

                await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                    @event.Id, _appConfig?.CurrentValue.AppId ?? string.Empty, @event);
                LocalQueueProcessor.Default.AddJobs(new IntegrationEventLogItem(@event.Id, @event.Topic, @event));
                await _eventLogService.MarkEventAsFailedAsync(@event.Id);
            }
        }
        else
        {
            _logger?.LogDebug(
                   "----- Publishing integration event (don't use local message): {IntegrationEventIdPublished} from {AppId} - ({IntegrationEvent})", @event.Id,
                   _appConfig?.CurrentValue.AppId ?? string.Empty, @event);

            await _dapr.PublishEventAsync(_daprPubsubName, topicName, (dynamic)@event);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException(nameof(IUnitOfWork), "You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
