namespace Masa.Contrib.Isolation.UoW.EF.Middleware;

public class IsolationMiddleware<TEvent> : IMiddleware<TEvent> where TEvent : IEvent
{
    private readonly IEnumerable<IIsolationMiddleware> _middlewares;

    public IsolationMiddleware(IEnumerable<IIsolationMiddleware> middlewares)
    {
        _middlewares = middlewares;
    }

    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        foreach (var middleware in _middlewares)
        {
            await middleware.HandleAsync();
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
            await middleware.HandleAsync();
        }

        await _next(httpContext);
    }
}
