// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventBus : IIntegrationEventBus
{
    private readonly Lazy<IEventBus?> _lazyEventBus;

    private IEventBus? EventBus => _lazyEventBus.Value;

    private readonly IPublisher _publisher;
    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly IIntegrationEventLogService? _eventLogService;
    private readonly IOptionsMonitor<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly IUnitOfWork? _unitOfWork;

    public IntegrationEventBus(
        Lazy<IEventBus?> eventBusLazy,
        IPublisher publisher,
        IIntegrationEventLogService? eventLogService = null,
        IOptionsMonitor<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<IntegrationEventBus>? logger = null,
        IUnitOfWork? unitOfWork = null)
    {
        _lazyEventBus = eventBusLazy;
        _publisher = publisher;
        _eventLogService = eventLogService;
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await PublishIntegrationAsync(integrationEvent, cancellationToken);
        }
        else if (EventBus != null)
        {
            await EventBus.PublishAsync(@event, cancellationToken);
        }
        else
        {
            throw new NotSupportedException(nameof(@event));
        }
    }

    private async Task PublishIntegrationAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent
    {
        if (@event.UnitOfWork == null && _unitOfWork != null)
            @event.UnitOfWork = _unitOfWork;

        var topicName = @event.Topic;
        if (@event.UnitOfWork is { UseTransaction: true } && _eventLogService != null)
        {
            _logger?.LogDebug("----- Saving changes and integrationEvent: {IntegrationEventId}", @event.GetEventId());
            await _eventLogService.SaveEventAsync(@event, @event.UnitOfWork.Transaction, cancellationToken);

#pragma warning disable S1135
            //todo: Status changes will be notified by local messaging services after subsequent upgrades
            @event.UnitOfWork.CommitState = CommitState.UnCommited;
#pragma warning disable S1135
        }
        else
        {
            _logger?.LogDebug(
                "----- Publishing integration event (don't use local message): {IntegrationEventIdPublished} from {AppId} - ({IntegrationEvent})",
                @event.GetEventId(),
                _masaAppConfigureOptions?.CurrentValue.AppId ?? string.Empty, @event);

            await _publisher.PublishAsync(topicName, (dynamic)@event, cancellationToken);
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException(nameof(IUnitOfWork), "You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
