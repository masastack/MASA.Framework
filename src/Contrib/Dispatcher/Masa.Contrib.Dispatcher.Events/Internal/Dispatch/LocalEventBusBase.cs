// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal abstract class LocalEventBusBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDispatchNetworkRoot _dispatchNetworkRoot;
    protected ILogger? Logger { get; }

    private readonly Lazy<IExecutionStrategy> _executionStrategyLazy;
    protected IExecutionStrategy ExecutionStrategy => _executionStrategyLazy.Value;

    protected LocalEventBusBase(
        IServiceProvider serviceProvider,
        IDispatchNetworkRoot dispatchNetworkRoot,
        ILogger? logger)
    {
        _serviceProvider = serviceProvider;
        _dispatchNetworkRoot = dispatchNetworkRoot;
        Logger = logger;
        _executionStrategyLazy = new Lazy<IExecutionStrategy>(serviceProvider.GetRequiredService<IExecutionStrategy>);
    }

    protected bool TryGetDispatchNetworks<TEvent>(
        TEvent @event,
        [NotNullWhen(true)] out List<DispatchRelationOptions>? dispatchRelations)
        where TEvent : IEvent
    {
        MasaArgumentException.ThrowIfNull(@event);

        var eventType = @event.GetType();

        return _dispatchNetworkRoot.DispatchNetworks.TryGetValue(eventType, out dispatchRelations);
    }

    protected bool TryGetCancelHandlerNetworks<TEvent>(
        TEvent @event,
        [NotNullWhen(true)] out List<EventHandlerAttribute>? cancelHandlers)
        where TEvent : IEvent
    {
        MasaArgumentException.ThrowIfNull(@event);

        var eventType = @event.GetType();

        return _dispatchNetworkRoot.CancelHandlerNetworks.TryGetValue(eventType, out cancelHandlers);
    }

    protected async Task<(bool IsSucceed, Exception? CancelException)> ExecuteCancelHandlerAsync<TEvent>(
        IEnumerable<EventHandlerAttribute> cancelHandlers,
        TEvent localEvent,
        CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        bool isSucceed = true;
        Exception? cancelException = null;
        var strategyOptions = new StrategyOptions();
        foreach (var cancelHandler in cancelHandlers)
        {
            if (cancelException != null)
                return (isSucceed, cancelException);

            strategyOptions.SetStrategy(cancelHandler);
            await ExecutionStrategy.ExecuteAsync(strategyOptions, localEvent, async @event =>
            {
                Logger?.LogDebug("----- Publishing event {@Event} rollback start: event id: {eventId} -----",
                    @event,
                    @event.GetEventId());
                await cancelHandler.ExecuteAction(_serviceProvider, @event, cancellationToken);
            }, (@event, ex, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                {
                    isSucceed = false;
                    cancelException = ex;
                }

                Logger?.LogError("----- Publishing event {@Event} rollback error ignored: event id: {eventId} -----",
                    @event,
                    @event.GetEventId());
                return Task.CompletedTask;
            });
        }
        return (isSucceed, cancelException);
    }
}
