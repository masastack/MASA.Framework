// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class IntegrationEventBus : IIntegrationEventBus
{
    private readonly Lazy<IEventBus?> _eventBusLazy;

    private IEventBus? EventBus => _eventBusLazy.Value;

    private readonly Lazy<IPublisher> _publisherLazy;
    private IPublisher Publisher => _publisherLazy.Value;

    private readonly ILogger<IntegrationEventBus>? _logger;
    private readonly IIntegrationEventLogService? _eventLogService;
    private readonly IOptions<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly IUnitOfWork? _unitOfWork;
    private readonly IsolationOptions? _isolationOptions;
    private readonly IMultiTenantContext? _multiTenantContext;
    private readonly IMultiEnvironmentContext? _multiEnvironmentContext;

    public IntegrationEventBus(
        IServiceProvider serviceProvider,
        Lazy<IEventBus?> eventBusLazy,
        Lazy<IPublisher> publisherLazy,
        IIntegrationEventLogService? eventLogService = null,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<IntegrationEventBus>? logger = null,
        IUnitOfWork? unitOfWork = null)
    {
        _eventBusLazy = eventBusLazy;
        _publisherLazy = publisherLazy;
        _eventLogService = eventLogService;
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _isolationOptions = serviceProvider.GetService<IOptions<IsolationOptions>>()?.Value;
        _multiTenantContext = serviceProvider.GetService<IMultiTenantContext>();
        _multiEnvironmentContext = serviceProvider.GetService<IMultiEnvironmentContext>();
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

    private async Task PublishIntegrationAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : IIntegrationEvent
    {
        if (@event.UnitOfWork == null && _unitOfWork != null)
            @event.UnitOfWork = _unitOfWork;

        var integrationEventMessageExpand = TryAddIntegrationEventMessageExpand();

        var topicName = @event.Topic;
        if (@event.UnitOfWork is { UseTransaction: true } && _eventLogService != null)
        {
            _logger?.LogDebug("----- Saving changes and integrationEvent: {IntegrationEventId}", @event.GetEventId());
            await _eventLogService.SaveEventAsync(@event, integrationEventMessageExpand, @event.UnitOfWork.Transaction, cancellationToken);

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
                _masaAppConfigureOptions?.Value.AppId ?? string.Empty, @event);

            await Publisher.PublishAsync(topicName, (dynamic)@event, integrationEventMessageExpand, cancellationToken);
        }
    }

    internal IntegrationEventExpand? TryAddIntegrationEventMessageExpand()
    {
        if (!(_isolationOptions?.Enable ?? false))
            return null;

        var messageExpand = new IntegrationEventExpand()
        {
            Isolation = new()
        };
        if (_isolationOptions.EnableMultiTenant && !(_multiTenantContext!.CurrentTenant?.Id ?? null).IsNullOrWhiteSpace())
        {
            messageExpand.Isolation.Add(_isolationOptions.MultiTenantIdName, _multiTenantContext.CurrentTenant!.Id!);
        }

        if (_isolationOptions.EnableMultiEnvironment && !_multiEnvironmentContext!.CurrentEnvironment.IsNullOrWhiteSpace())
        {
            messageExpand.Isolation.Add(_isolationOptions.MultiEnvironmentName, _multiEnvironmentContext.CurrentEnvironment);
        }

        return messageExpand;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork is null)
            throw new ArgumentNullException(nameof(IUnitOfWork), "You need to UseUoW when adding services");

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
