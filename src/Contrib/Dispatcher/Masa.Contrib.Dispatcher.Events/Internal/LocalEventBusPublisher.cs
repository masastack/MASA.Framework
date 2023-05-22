// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal;

internal class LocalEventBusPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDispatchNetworkRoot _dispatchNetworkRoot;
    private readonly ILogger<LocalEventBusPublisher>? _logger;

    public LocalEventBusPublisher(
        IServiceProvider serviceProvider,
        IDispatchNetworkRoot dispatchNetworkRoot,
        ILogger<LocalEventBusPublisher>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _dispatchNetworkRoot = dispatchNetworkRoot;
        _logger = logger;
    }

    public  Task PublishEventAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (!TryGetDispatchNetworks(@event, out List<DispatchRelationOptions>? dispatchNetworks))
        {
            var eventType = @event.GetType();
            _logger?.LogError(
                "Dispatcher: The [{EventTypeFullName}] Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus",
                eventType.FullName);
            throw new UserFriendlyException($"The {eventType.FullName} Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus");
        }

        return ExecuteEventHandlerAsync(dispatchNetworks, @event, cancellationToken);
    }

    private bool  TryGetDispatchNetworks<TEvent>(TEvent @event,
        [NotNullWhen(true)] out List<DispatchRelationOptions>? dispatchRelations)
        where TEvent : IEvent
    {
        MasaArgumentException.ThrowIfNull(@event);

        var eventType = @event.GetType();

        return _dispatchNetworkRoot.DispatchNetwork.TryGetValue(eventType, out dispatchRelations);
    }

    private async Task ExecuteEventHandlerAsync<TEvent>(
        List<DispatchRelationOptions> dispatchRelations,
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        var executionStrategy = _serviceProvider.GetRequiredService<IExecutionStrategy>();
        var strategyOptions = new StrategyOptions();
        bool isCancel = false;
        foreach (var dispatchRelation in dispatchRelations)
        {
            if (isCancel) return;

            strategyOptions.SetStrategy(dispatchRelation.Handler);

            await executionStrategy.ExecuteAsync(strategyOptions, @event, async @event =>
            {
                _logger?.LogDebug("----- Publishing event {@Event}: message id: {messageId} -----", @event, @event.GetEventId());

                await dispatchRelation.Handler.ExecuteAction(_serviceProvider, @event, cancellationToken);

            }, async (@event, ex, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                {
                    isCancel = true;
                    if (dispatchRelation.CancelHandlers.Any())
                        await ExecuteEventCanceledHandlerAsync(executionStrategy, dispatchRelation.CancelHandlers,
                            @event, cancellationToken);
                    else
                        ex.ThrowException();
                }
                else
                {
                    _logger?.LogError("----- Publishing event {@Event} error rollback is ignored: message id: {messageId} -----", @event,
                        @event.GetEventId());
                }
            });
        }
    }

    private async Task ExecuteEventCanceledHandlerAsync<TEvent>(
        IExecutionStrategy executionStrategy,
        IEnumerable<EventHandlerAttribute> cancelHandlers,
        TEvent @event,
        CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        StrategyOptions strategyOptions = new StrategyOptions();
        foreach (var cancelHandler in cancelHandlers)
        {
            strategyOptions.SetStrategy(cancelHandler);
            await executionStrategy.ExecuteAsync(strategyOptions, @event, async @event =>
            {
                _logger?.LogDebug("----- Publishing event {@Event} rollback start: message id: {messageId} -----", @event,
                    @event.GetEventId());
                await cancelHandler.ExecuteAction(_serviceProvider, @event, cancellationToken);
            }, (@event, ex, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                    ex.ThrowException();

                _logger?.LogError("----- Publishing event {@Event} rollback error ignored: message id: {messageId} -----", @event,
                    @event.GetEventId());
                return Task.CompletedTask;
            });
        }
    }
}
