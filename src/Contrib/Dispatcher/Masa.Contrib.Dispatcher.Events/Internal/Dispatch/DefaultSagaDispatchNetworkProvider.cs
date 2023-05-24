// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class DefaultSagaDispatchNetworkProvider : IDispatchNetworkProvider
{
    private readonly Assembly[] _assemblies;
    private readonly List<EventHandlerAttribute> _handlerNetwork;
    private readonly List<EventHandlerAttribute> _cancelHandlerNetwork;

    public IReadOnlyList<EventHandlerAttribute> HandlerNetwork => _handlerNetwork;

    public IReadOnlyList<EventHandlerAttribute> CancelHandlerNetwork => _cancelHandlerNetwork;

    public DefaultSagaDispatchNetworkProvider(Assembly[] assemblies, ILoggerFactory? loggerFactory)
    {
        _assemblies = assemblies;
        _handlerNetwork = new();
        _cancelHandlerNetwork = new();

        BuildEventHandlerList(typeof(IEventHandler<>));
        BuildEventHandlerList(typeof(ISagaEventHandler<>));
    }

    private void BuildEventHandlerList(Type eventHandlerGenericType)
    {
        foreach (var eventHandlerInstanceType in GetEventHandlerInstanceTypeList(eventHandlerGenericType))
        {
            BuildEventHandlerListByInstance(eventHandlerInstanceType);
        }
    }

    private void BuildEventHandlerListByInstance(Type eventHandlerInstanceType)
    {
        var eventHandlers = GetEventHandlerList(eventHandlerInstanceType);

        var eventHandler = eventHandlers.FirstOrDefault(x => x.Order != int.MaxValue);
        var actualOrder = eventHandler?.Order ?? int.MaxValue;

        foreach (var handler in eventHandlers)
        {
            if ((handler.IsCancel &&
                    _cancelHandlerNetwork.Any(cancelHandler => cancelHandler.ActionMethodInfo == handler.ActionMethodInfo)) ||
                (!handler.IsCancel && _handlerNetwork.Any(attr => attr.ActionMethodInfo == handler.ActionMethodInfo)))
            {
                continue;
            }

            if (actualOrder != handler.Order) handler.Order = actualOrder;

            if (!handler.IsCancel) _handlerNetwork.Add(handler);
            else _cancelHandlerNetwork.Add(handler);
        }
    }

    private List<EventHandlerAttribute> GetEventHandlerList(Type eventHandlerInstanceType)
    {
        var methods = eventHandlerInstanceType.GetMethods();

        var eventHandlers = new List<EventHandlerAttribute>();
        foreach (var method in methods)
        {
            if (!DispatchNetworkUtils.IsSagaMode(eventHandlerInstanceType, method))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 2 || (!typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType) ||
                    typeof(IIntegrationEvent).IsAssignableFrom(parameters[0].ParameterType)))
                continue;

            var attribute = method.GetCustomAttributes(typeof(EventHandlerAttribute), true).FirstOrDefault();
            var handler = attribute as EventHandlerAttribute;

            handler ??= new EventHandlerAttribute();
            handler.ActionMethodInfo = method;
            handler.InstanceType = eventHandlerInstanceType;
            handler.EventType = parameters.Select(x => x.ParameterType).FirstOrDefault()!;
            handler.IsCancel = method.Name.Equals(nameof(ISagaEventHandler<IEvent>.CancelAsync));
            handler.BuildExpression();
            eventHandlers.Add(handler);
        }

        //In saga mode, when the user sets an Order for either HandlerAsync or CancelAsync
        //the default int.MaxValue Order will be overridden
        //but when the Order of HandlerAsync and CancelAsync are inconsistent
        //an error will be displayed
        if (eventHandlers.Where(handler => handler.Order != int.MaxValue).Select(handler => handler.Order).Distinct().Count() > 1)
        {
            throw new ArgumentException(
                $"In saga mode {nameof(IEventHandler<IEvent>.HandleAsync)} needs to be the same as {nameof(ISagaEventHandler<IEvent>.CancelAsync)} Order");
        }
        return eventHandlers;
    }

    internal List<Type> GetEventHandlerInstanceTypeList(Type eventHandlerGenericType)
    {
        var types = _assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsConcrete() && type.IsImplementerOfGeneric(eventHandlerGenericType))
            .ToList();
        return types;
    }
}
