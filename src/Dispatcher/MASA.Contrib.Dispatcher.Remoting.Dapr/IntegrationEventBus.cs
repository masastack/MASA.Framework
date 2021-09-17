namespace MASA.Contrib.Dispatcher.Remoting.Dapr;

public class IntegrationEventBus : IIntegrationEventBus
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly IOptionsMonitor<AppConfig> _appConfig;
    private readonly string _daprPubsubName;

    public IntegrationEventBus(DaprClient dapr, IIntegrationEventLogService eventLogService, IOptionsMonitor<AppConfig> appConfig, ILogger<IntegrationEventBus>? logger)
    {
        _dapr = dapr;
        _eventLogService = eventLogService;
        _appConfig = appConfig;
        _logger = logger;
        _daprPubsubName = !string.IsNullOrWhiteSpace(ServiceCollectionExtensions.GlobalDaprPubsubName) ? ServiceCollectionExtensions.GlobalDaprPubsubName : DAPR_PUBSUB_NAME;
    }

    public async Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IIntegrationEvent
    {
        try
        {
            _logger?.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppId} - ({@IntegrationEvent})", @event.Id, _appConfig.CurrentValue.AppId, @event);

            await _eventLogService.MarkEventAsInProgressAsync(@event.Id);

            var topicName = @event.GetType().Name;
            _logger?.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", @event, _daprPubsubName, topicName);
            await _dapr.PublishEventAsync(_daprPubsubName, topicName, (dynamic)@event);

            await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})", @event.Id, _appConfig.CurrentValue.AppId, @event);
            await _eventLogService.MarkEventAsFailedAsync(@event.Id);
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, DbTransaction transaction)
        where TEvent : IIntegrationEvent
    {
        try
        {
            _logger?.LogInformation("----- Saving changes and integrationEvent: {IntegrationEventId}", @event.Id);

            await _eventLogService.SaveEventAsync(@event, transaction);

            _logger?.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppId} - ({@IntegrationEvent})", @event.Id, _appConfig.CurrentValue.AppId, @event);

            await _eventLogService.MarkEventAsInProgressAsync(@event.Id);

            var topicName = @event.GetType().Name;
            _logger?.LogInformation("Publishing event {@Event} to {PubsubName}.{TopicName}", @event, _daprPubsubName, topicName);
            await _dapr.PublishEventAsync(_daprPubsubName, topicName, (dynamic)@event);

            await _eventLogService.MarkEventAsPublishedAsync(@event.Id);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})", @event.Id, _appConfig.CurrentValue.AppId, @event);
            await _eventLogService.MarkEventAsFailedAsync(@event.Id);
        }
    }
}