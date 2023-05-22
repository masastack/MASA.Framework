// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class DispatchNetworkRoot : IDispatchNetworkRoot
{
    public IReadOnlyDictionary<Type, List<DispatchRelationOptions>> DispatchNetwork { get; }

    public DispatchNetworkRoot(List<IDispatchNetworkProvider> providers)
        => DispatchNetwork = BuildNetwork(providers);

    private static Dictionary<Type, List<DispatchRelationOptions>> BuildNetwork(List<IDispatchNetworkProvider> providers)
    {
        return BuildNetwork(
            providers.SelectMany(provider => provider.HandlerNetwork).ToList(),
            providers.SelectMany(provider => provider.CancelHandlerNetwork).ToList());
    }

    private static Dictionary<Type, List<DispatchRelationOptions>> BuildNetwork(
        IReadOnlyList<EventHandlerAttribute> handlerNetworks,
        IReadOnlyList<EventHandlerAttribute> cancelHandlerNetworks)
    {
        var dispatchNetwork = new Dictionary<Type, List<DispatchRelationOptions>>();

        foreach (var eventType in handlerNetworks.Select(attr => attr.EventType).Distinct())
        {
            var list = new List<DispatchRelationOptions>();
            foreach (var handlerNetwork in handlerNetworks.Where(attr => attr.EventType == eventType))
            {
                var dispatchRelationOptions = new DispatchRelationOptions(handlerNetwork)
                {
                    CancelHandlers = cancelHandlerNetworks.Where(attr =>
                        attr.EventType == eventType &&
                        attr.IsCancel &&
                        ((attr.Order < handlerNetwork.Order && handlerNetwork.FailureLevels == FailureLevels.Throw) ||
                            attr.Order <= handlerNetwork.Order && handlerNetwork.FailureLevels == FailureLevels.ThrowAndCancel))
                        .OrderBy(attr=> attr.Order)
                        .Reverse()
                };

                list.Add(dispatchRelationOptions);
            }

            dispatchNetwork.Add(eventType, list);
        }
        return dispatchNetwork;
    }
}
