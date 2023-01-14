// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class MiddlewareEventHandler
{
    [EventHandler]
    public void Handle(MiddlewareEvent @event)
    {
        @event.Results.Add(nameof(MiddlewareEventHandler));
    }
}
