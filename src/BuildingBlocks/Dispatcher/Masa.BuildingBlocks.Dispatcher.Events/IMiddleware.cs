// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public delegate Task EventHandlerDelegate();

/// <summary>
/// Middleware is assembled into an event pipeline to handle invoke event and result
/// </summary>
public interface IMiddleware<in TEvent>
    where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, EventHandlerDelegate next);

    /// <summary>
    /// If Recursive is not supported, the current Middleware only executes once
    /// If Recursive is supported, Middleware will be executed everytime when EventBus is nested
    /// </summary>
    bool SupportRecursive { get; }
}
