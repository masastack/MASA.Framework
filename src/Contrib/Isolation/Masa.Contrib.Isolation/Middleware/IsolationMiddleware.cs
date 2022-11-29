// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Middleware;

public class IsolationMiddleware<TEvent> : Middleware<TEvent> where TEvent : IEvent
{
    private readonly IEnumerable<IIsolationMiddleware> _middlewares;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IsolationMiddleware(IEnumerable<IIsolationMiddleware> middlewares, IHttpContextAccessor httpContextAccessor)
    {
        _middlewares = middlewares;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        foreach (var middleware in _middlewares)
        {
            await middleware.HandleAsync(_httpContextAccessor.HttpContext);
        }

        await next();
    }
}

public class IsolationMiddleware
{
    private readonly RequestDelegate _next;

    public IsolationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IEnumerable<IIsolationMiddleware> middlewares)
    {
        foreach (var middleware in middlewares)
        {
            await middleware.HandleAsync(httpContext);
        }

        await _next(httpContext);
    }
}
