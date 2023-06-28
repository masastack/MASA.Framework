// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class LocalEventBus : LocalEventBusBase, ILocalEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<IExecuteProvider> _executeProviderLazy;
    private IExecuteProvider ExecuteProvider => _executeProviderLazy.Value;

    public LocalEventBus(
        IServiceProvider serviceProvider,
        IDispatchNetworkRoot dispatchNetworkRoot,
        ILogger<LocalEventBus>? logger = null) : base(serviceProvider, dispatchNetworkRoot, logger)
    {
        _serviceProvider = serviceProvider;
        _executeProviderLazy = new Lazy<IExecuteProvider>(serviceProvider.GetRequiredService<IExecuteProvider>);
    }

    public Task ExecuteHandlerAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (TryGetDispatchNetworks(@event, out List<DispatchRelationOptions>? dispatchNetworks))
            return ExecuteEventHandlerAsync(dispatchNetworks, @event, cancellationToken);

        var eventType = @event.GetType();
        Logger?.LogError(
            "Dispatcher: The [{EventTypeFullName}] Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus",
            eventType.FullName);
        throw new UserFriendlyException(
            $"The {eventType.FullName} Handler method was not found. Check to see if the EventHandler feature is added to the method and if the Assembly is specified when using EventBus");
    }

    /// <summary>
    /// Executes cancellation handlers for the specified event
    /// </summary>
    /// <returns></returns>
    public Task ExecuteCancelHandlerAsync<TEvent>(
        TEvent @event,
        List<EventHandlerAttribute> cancelHandlers,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent
        => base.ExecuteCancelHandlerAsync(cancelHandlers, @event, cancellationToken);

    public async Task ExecuteAllCancelHandlerAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (TryGetCancelHandlerNetworks(@event, out List<EventHandlerAttribute>? cancelHandlers))
        {
            //When a cancellation handler exists
            var cancelHandlerResult = await ExecuteCancelHandlerAsync(cancelHandlers, @event, cancellationToken);
            var executeInfo = ExecuteProvider.ExecuteResult;
            if (cancelHandlerResult.IsSucceed)
            {
                executeInfo.Status = ExecuteStatus.RollbackSucceeded;
            }
            else
            {
                MasaArgumentException.ThrowIfNull(executeInfo.Exception);

                executeInfo.Exception.Data.Add("Cancel handler exception", cancelHandlerResult.CancelException);
                executeInfo.Status = ExecuteStatus.RollbackFailed;
            }
            ExecuteProvider.SetExecuteResult(executeInfo);
        }
    }

    private async Task ExecuteEventHandlerAsync<TEvent>(
        List<DispatchRelationOptions> dispatchRelations,
        TEvent localEvent,
        CancellationToken cancellationToken)
        where TEvent : IEvent
    {
        var strategyOptions = new StrategyOptions();
        var isCancel = false;
        EventExecuteInfo? eventExecuteInfo = null;
        foreach (var dispatchRelation in dispatchRelations)
        {
            if (isCancel) return;

            strategyOptions.SetStrategy(dispatchRelation.Handler);

            await ExecutionStrategy.ExecuteAsync(strategyOptions, localEvent, async @event =>
            {
                Logger?.LogDebug("----- Publishing event {@Event}: event id: {eventId} -----", @event, @event.GetEventId());

                await dispatchRelation.Handler.ExecuteAction(_serviceProvider, @event, cancellationToken);
            }, async (@event, handlerException, failureLevels) =>
            {
                if (failureLevels != FailureLevels.Ignore)
                {
                    isCancel = true;
                    eventExecuteInfo = new EventExecuteInfo()
                    {
                        Exception = handlerException
                    };

                    (bool IsSucceed, Exception? CancelException)? cancelHandlerResult = null;
                    if (dispatchRelation.CancelHandlers.Any())
                    {
                        cancelHandlerResult = await ExecuteCancelHandlerAsync(
                            dispatchRelation.CancelHandlers,
                            @event,
                            cancellationToken);
                    }

                    switch (cancelHandlerResult)
                    {
                        case { IsSucceed: false }:
                            eventExecuteInfo.Exception.Data.Add("cancel handler exception", cancelHandlerResult.Value.CancelException);
                            eventExecuteInfo.Status = ExecuteStatus.RollbackFailed;
                            break;
                        case { IsSucceed: true }:
                            eventExecuteInfo.Status = ExecuteStatus.RollbackSucceeded;
                            break;
                        default:
                            eventExecuteInfo.Status = ExecuteStatus.Failed;
                            break;
                    }
                }
                else
                {
                    // Publish event fails, but errors are ignored

                    Logger?.LogError(
                        "----- Publishing event {@Event} error is ignored: event id: {EventId}, current instance name: {InstanceName}, current method name: {MethodName} -----",
                        @event,
                        @event.GetEventId(),
                        dispatchRelation.Handler.InstanceType.FullName ?? dispatchRelation.Handler.InstanceType.Name,
                        dispatchRelation.Handler.ActionMethodInfo.Name);
                }
            });

            eventExecuteInfo ??= new EventExecuteInfo()
            {
                Status = ExecuteStatus.Succeed
            };
            ExecuteProvider.SetExecuteResult(eventExecuteInfo);
            eventExecuteInfo.Exception?.ThrowException();
        }
    }
}
