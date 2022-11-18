// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventBus : IIntegrationEventBus
{
    private readonly DispatcherOptions _dispatcherOptions;
    private readonly IPublisher _publisher;
    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly IIntegrationEventLogService? _eventLogService;
    private readonly IEventBus? _eventBus;
    private readonly IUnitOfWork? _unitOfWork;

    public IntegrationEventBus(IOptions<DispatcherOptions> options,
        IPublisher publisher,
        IIntegrationEventLogService? eventLogService = null,
        ILogger<IntegrationEventBus>? logger = null,
        IEventBus? eventBus = null,
        IUnitOfWork? unitOfWork = null)
    {
        _dispatcherOptions = options.Value;
        _publisher = publisher;
        _eventLogService = eventLogService;
        _logger = logger;
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
        if (@event.UnitOfWork is { UseTransaction: true } && _eventLogService != null)
        {
            // Using outbox mode
            _logger?.LogDebug("----- Saving changes Integration Event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                @event.GetEventId(), MasaAppConfig.AppId(), @event);
            await _eventLogService.SaveEventAsync(@event, @event.UnitOfWork!.Transaction);
        }
        else
        {
            _logger?.LogDebug(
                "----- Publishing integration event (don't use local message): {IntegrationEventIdPublished} from {AppId} - ({@IntegrationEvent})",
                @event.GetEventId(), MasaAppConfig.AppId(), @event);

            await _publisher.PublishAsync(topicName, (dynamic)@event);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException(nameof(IUnitOfWork), "You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }


}
