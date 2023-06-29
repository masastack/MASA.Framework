// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class DispatchNetworkRoot : IDispatchNetworkRoot
{
    public IReadOnlyDictionary<Type, List<DispatchRelationOptions>> DispatchNetworks { get; }

    public IReadOnlyDictionary<Type, List<EventHandlerAttribute>> CancelHandlerNetworks { get; }

    public DispatchNetworkRoot(List<IDispatchNetworkProvider> providers)
    {
        DispatchNetworks = BuildDispatchNetworks(providers);
        CancelHandlerNetworks = BuildCancelHandlerNetworks(providers);
    }

    private static Dictionary<Type, List<DispatchRelationOptions>> BuildDispatchNetworks(List<IDispatchNetworkProvider> providers)
    {
        return BuildDispatchNetworks(
            providers.SelectMany(provider => provider.HandlerNetwork).OrderBy(provider => provider.Order).ToList(),
            providers.SelectMany(provider => provider.CancelHandlerNetwork).ToList());
    }

    private static Dictionary<Type, List<DispatchRelationOptions>> BuildDispatchNetworks(
        IReadOnlyList<EventHandlerAttribute> handlers,
        IReadOnlyList<EventHandlerAttribute> cancelHandlers)
    {
        var dispatchNetwork = new Dictionary<Type, List<DispatchRelationOptions>>();

        foreach (var eventType in handlers.Select(attr => attr.EventType).Distinct())
        {
            var list = handlers.Where(attr => attr.EventType == eventType)
                .Select(handlerNetwork => new DispatchRelationOptions(handlerNetwork)
                {
                    CancelHandlers = cancelHandlers
                        .Where(attr => attr.EventType == eventType && attr.IsCancel &&
                            ((attr.Order < handlerNetwork.Order && handlerNetwork.FailureLevels == FailureLevels.Throw) ||
                                attr.Order <= handlerNetwork.Order && handlerNetwork.FailureLevels == FailureLevels.ThrowAndCancel))
                        .OrderBy(attr => attr.Order)
                        .Reverse()
                })
                .ToList();

            dispatchNetwork.Add(eventType, list);
        }
        return dispatchNetwork;
    }

    private static Dictionary<Type, List<EventHandlerAttribute>> BuildCancelHandlerNetworks(List<IDispatchNetworkProvider> providers)
    {
        return BuildCancelHandlerNetworks(providers.SelectMany(provider => provider.CancelHandlerNetwork).ToList());
    }

    private static Dictionary<Type, List<EventHandlerAttribute>> BuildCancelHandlerNetworks(List<EventHandlerAttribute> cancelHandlers)
    {
        var cancelHandlerNetworks = new Dictionary<Type, List<EventHandlerAttribute>>();

        foreach (var eventType in cancelHandlers.Select(attr => attr.EventType).Distinct())
        {
            var cancelHandlersTemp = cancelHandlers.Where(attr => attr.EventType == eventType).ToList();
            cancelHandlerNetworks.Add(eventType, cancelHandlersTemp);
        }

        return cancelHandlerNetworks;
    }
}
