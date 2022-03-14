namespace Masa.Contrib.Dispatcher.Events;

public static class UseMiddlewareExtensions
{
    public static EventBusBuilder UseMiddleware(
        this EventBusBuilder eventBusBuilder,
        Type middleware,
        ServiceLifetime middlewareLifetime = ServiceLifetime.Scoped)
    {
        if (!typeof(IMiddleware<>).IsGenericInterfaceAssignableFrom(middleware))
            throw new ArgumentException($"{middleware.Name} doesn't implement IMiddleware<>");

        var descriptor = new ServiceDescriptor(typeof(IMiddleware<>), middleware, middlewareLifetime);
        eventBusBuilder.Services.TryAddEnumerable(descriptor);
        return eventBusBuilder;
    }
}
