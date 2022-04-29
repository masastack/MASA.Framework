// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal.Dispatch;

internal class DispatcherBase
{
    protected static DispatchRelationNetwork? SharingRelationNetwork;

    protected readonly IServiceCollection Services;

    protected readonly Assembly[] Assemblies;

    protected readonly ILogger<DispatcherBase>? Logger;

    public DispatcherBase(IServiceCollection services, Assembly[] assemblies, bool forceInit)
    {
        Services = services;
        Assemblies = assemblies;
        var serviceProvider = services.BuildServiceProvider();
        if (SharingRelationNetwork == null || forceInit)
        {
            SharingRelationNetwork = new DispatchRelationNetwork(serviceProvider.GetService<ILogger<DispatchRelationNetwork>>());
        }
        Logger = serviceProvider.GetService<ILogger<DispatcherBase>>();
    }

    public async Task PublishEventAsync<TEvent>(IServiceProvider serviceProvider, TEvent @event)
        where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(@event, nameof(@event));
        var eventType = @event.GetType();
        if (!SharingRelationNetwork!.RelationNetwork.TryGetValue(eventType, out List<DispatchRelationOptions>? dispatchRelations))
        {
            if (@event is IIntegrationEvent)
            {
                Logger?.LogError($"Dispatcher: The current event is an out-of-process event. You should use IIntegrationEventBus or IDomainEventBus to send it");
                throw new ArgumentNullException($"The current event is an out-of-process event. You should use IIntegrationEventBus or IDomainEventBus to send it");
            }

            Logger?.LogError($"Dispatcher: The {eventType.FullName} Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus");
            throw new ArgumentNullException($"The {eventType.FullName} Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus");
        }
        await ExecuteEventHandlerAsync(serviceProvider, dispatchRelations, @event);
    }

    private async Task ExecuteEventHandlerAsync<TEvent>(IServiceProvider serviceProvider,
        List<DispatchRelationOptions> dispatchRelations,
        TEvent @event)
        where TEvent : IEvent
    {
        var executionStrategy = serviceProvider.GetRequiredService<IExecutionStrategy>();
        StrategyOptions strategyOptions = new StrategyOptions();
        bool isCancel = false;
        EventHandlerAttribute dispatchHandler;
        foreach (var dispatchRelation in dispatchRelations)
        {
            if (isCancel) return;
            dispatchHandler = dispatchRelation.Handler;

            strategyOptions.SetStrategy(dispatchHandler);

            await executionStrategy.ExecuteAsync(strategyOptions, @event, async (@event) =>
            {
                Logger?.LogDebug("----- Publishing event {@Event}: message id: {messageId} -----", @event, @event.GetEventId());
                await dispatchHandler.ExcuteAction(serviceProvider, @event);
            }, async (@event, ex, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                {
                    isCancel = true;
                    if (dispatchRelation.CancelHandlers.Any())
                        await ExecuteEventCanceledHandlerAsync(serviceProvider, Logger, executionStrategy, dispatchRelation.CancelHandlers, @event);
                    else
                        ex.ThrowException();
                }
                else
                {
                    Logger?.LogError("----- Publishing event {@Event} error rollback is ignored: message id: {messageId} -----", @event, @event.GetEventId());
                }
            });
        }
    }

    private async Task ExecuteEventCanceledHandlerAsync<TEvent>(IServiceProvider serviceProvider,
        ILogger<DispatcherBase>? logger,
        IExecutionStrategy executionStrategy,
        IEnumerable<EventHandlerAttribute> cancelHandlers,
        TEvent @event)
        where TEvent : IEvent
    {
        StrategyOptions strategyOptions = new StrategyOptions();
        foreach (var cancelHandler in cancelHandlers)
        {
            strategyOptions.SetStrategy(cancelHandler);
            await executionStrategy.ExecuteAsync(strategyOptions, @event, async @event =>
            {
                logger?.LogDebug("----- Publishing event {@Event} rollback start: message id: {messageId} -----", @event, @event.GetEventId());
                await cancelHandler.ExcuteAction(serviceProvider, @event);
            }, (@event, ex, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                    ex.ThrowException();

                logger?.LogError("----- Publishing event {@Event} rollback error ignored: message id: {messageId} -----", @event, @event.GetEventId());
                return Task.CompletedTask;
            });
        }
    }

    protected void AddRelationNetwork(Type parameterType, EventHandlerAttribute handler)
    {
        SharingRelationNetwork!.Add(parameterType, handler);
    }

    protected IEnumerable<Type> GetAddServiceTypeList() => SharingRelationNetwork!.HandlerRelationNetwork
        .Concat(SharingRelationNetwork.CancelRelationNetwork)
        .SelectMany(relative => relative.Value)
        .Where(dispatchHandler => dispatchHandler.InvokeDelegate != null)
        .Select(dispatchHandler => dispatchHandler.InstanceType).Distinct();

    protected void Build() => SharingRelationNetwork!.Build();

    protected bool IsSagaMode(Type handlerType, MethodInfo method) =>
      typeof(IEventHandler<>).IsGenericInterfaceAssignableFrom(handlerType) && method.Name.Equals(nameof(IEventHandler<IEvent>.HandleAsync)) ||
      typeof(ISagaEventHandler<>).IsGenericInterfaceAssignableFrom(handlerType) && method.Name.Equals(nameof(ISagaEventHandler<IEvent>.CancelAsync));
}
