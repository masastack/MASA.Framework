// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public abstract class Middleware<TEvent> : IMiddleware<TEvent> where TEvent : IEvent
{
    public virtual bool SupportRecursive => true;

    public abstract Task HandleAsync(TEvent @event, EventHandlerDelegate next);
}
