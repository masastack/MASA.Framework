// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests;

public class RequestEventMiddleware<TEvent> : EventMiddleware<TEvent> where TEvent : IEvent
{
    private readonly ILogger<RequestEventMiddleware<TEvent>>? _logger;
    public RequestEventMiddleware(ILogger<RequestEventMiddleware<TEvent>>? logger = null) => _logger = logger;

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var eventType = @event.GetType();
        _logger?.LogInformation("----- Handling command {FullName} ({event})", eventType.FullName, @event);
        await next();
    }
}
