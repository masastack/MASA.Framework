// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Infrastructure.Middleware;

public class RecordEventMiddleware<TEvent> : EventMiddleware<TEvent>
    where TEvent : IEvent
{
    public static int Time { get; set; }

    public override bool SupportRecursive => false;

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        Time++;
        await next();
    }
}
