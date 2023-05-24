// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.Events.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal static class DispatchNetworkUtils
{
    public static bool IsSagaMode(Type handlerType, MethodInfo method) =>
        (handlerType.IsImplementerOfGeneric(typeof(IEventHandler<>)) && method.Name.Equals(nameof(IEventHandler<IEvent>.HandleAsync))) ||
        (handlerType.IsImplementerOfGeneric(typeof(ISagaEventHandler<>)) && method.Name.Equals(nameof(ISagaEventHandler<IEvent>.CancelAsync)));

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
