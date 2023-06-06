// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal interface IDispatchNetworkRoot
{
    IReadOnlyDictionary<Type, List<DispatchRelationOptions>> DispatchNetworks { get; }

    IReadOnlyDictionary<Type,List<EventHandlerAttribute>> CancelHandlerNetworks { get; }
}
