// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Perfs.Extensions.Middleware;

public class LoggingMiddleware<TEvent> : EventMiddleware<TEvent> where TEvent : notnull, IEvent
{
    private readonly ILogger<LoggingMiddleware<TEvent>>? _logger;
    public LoggingMiddleware(ILogger<LoggingMiddleware<TEvent>>? logger = null) => _logger = logger;

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var eventType = @event.GetType();
        _logger?.LogInformation("----- Handling command {CommandName} ({@Command})", eventType.FullName, @event);

        await next();
    }
}
