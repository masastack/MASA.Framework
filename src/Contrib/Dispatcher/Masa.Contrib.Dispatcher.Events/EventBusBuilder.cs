// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public class EventBusBuilder : IEventBusBuilder
{
    public IServiceCollection Services { get; }

    public EventBusBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Use middleware (the order of middleware is executed in reverse order according to the order of addition)
    /// </summary>
    /// <param name="middlewareType">middleware types</param>
    /// <param name="middlewareLifetime">Middleware service life cycle</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IEventBusBuilder UseMiddleware(Type middlewareType, ServiceLifetime middlewareLifetime = ServiceLifetime.Transient)
    {
        if (!typeof(IMiddleware<>).IsGenericInterfaceAssignableFrom(middlewareType))
            throw new ArgumentException($"{middlewareType.Name} doesn't implement IMiddleware<>");

        var descriptor = new ServiceDescriptor(typeof(IMiddleware<>), middlewareType, middlewareLifetime);
        Services.TryAddEnumerable(descriptor);
        return this;
    }

    /// <summary>
    /// Use a collection of middleware (the order of middleware is executed in reverse order according to the order of addition)
    /// </summary>
    /// <param name="middlewareTypes">collection of middleware types</param>
    /// <param name="middlewareLifetime">Middleware service life cycle</param>
    /// <returns></returns>
    public IEventBusBuilder UseMiddleware(IEnumerable<Type> middlewareTypes, ServiceLifetime middlewareLifetime = ServiceLifetime.Transient)
    {
        foreach (var middlewareType in middlewareTypes) UseMiddleware(middlewareType, middlewareLifetime);
        return this;
    }
}
