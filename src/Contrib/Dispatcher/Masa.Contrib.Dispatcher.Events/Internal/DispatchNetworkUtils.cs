// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal static class DispatchNetworkUtils
{
    public static bool IsSagaMode(Type handlerType, MethodInfo method) =>
        typeof(IEventHandler<>).IsGenericInterfaceAssignableFrom(handlerType) &&
        method.Name.Equals(nameof(IEventHandler<IEvent>.HandleAsync)) ||
        typeof(ISagaEventHandler<>).IsGenericInterfaceAssignableFrom(handlerType) &&
        method.Name.Equals(nameof(ISagaEventHandler<IEvent>.CancelAsync));

    public static List<Type> GetServiceTypes(List<DispatchRelationOptions> dispatchRelationOptions)
    {
        var serviceTypes = new List<Type>(dispatchRelationOptions
            .SelectMany(options => options.CancelHandlers.Select(attr => attr.InstanceType))
            .Distinct()
            .ToList());
        serviceTypes.AddRange(dispatchRelationOptions.Select(options => options.Handler.InstanceType));
        return serviceTypes.Distinct().ToList();
    }
}
