// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal class DispatchRelationOptions
{
    public EventHandlerAttribute Handler { get; set; } = new();

    public IEnumerable<EventHandlerAttribute> CancelHandlers { get; set; } = new List<EventHandlerAttribute>();

    private DispatchRelationOptions() { }

    public DispatchRelationOptions(EventHandlerAttribute handler) : this() => Handler = handler;
}
