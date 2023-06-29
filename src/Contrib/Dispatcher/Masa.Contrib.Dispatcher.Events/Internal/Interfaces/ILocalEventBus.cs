// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

internal interface ILocalEventBus
{
    Task ExecuteHandlerAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Executes all cancellation handlers for the specified event
    /// </summary>
    /// <returns></returns>
    Task ExecuteAllCancelHandlerAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Executes cancellation handlers for the specified event
    /// </summary>
    /// <returns></returns>
    Task ExecuteCancelHandlerAsync<TEvent>(
        TEvent @event,
        List<EventHandlerAttribute> cancelHandlers,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
