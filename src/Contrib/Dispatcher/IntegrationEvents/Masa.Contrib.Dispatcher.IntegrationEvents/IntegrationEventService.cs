// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly IPublisher _publisher;
    private readonly ILogger<IntegrationEventService>? _logger;

    public IntegrationEventService(IIntegrationEventLogService eventLogService,
        IPublisher publisher,
        ILogger<IntegrationEventService>? logger = null)
    {
        _eventLogService = eventLogService;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId, CancellationToken stoppingToken = default)
    {
        var retrieveEventLogs = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId, stoppingToken);

        string appId = MasaAppConfig.AppId();
        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                _logger?.LogDebug("----- Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                    eventLog.EventId, appId, eventLog.Event);

                await _eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                _logger?.LogDebug("Publishing integration event {EventLog} to {TopicName}",
                    eventLog, eventLog.Event.Topic);

                await _publisher.PublishAsync(eventLog.Event.Topic, eventLog.Event, stoppingToken);

                await _eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention, no processing required
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                    eventLog.EventId, appId, eventLog);
                await _eventLogService.MarkEventAsFailedAsync(eventLog.EventId);
            }
        }
    }
}
