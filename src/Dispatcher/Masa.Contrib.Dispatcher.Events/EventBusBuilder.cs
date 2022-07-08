// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public class EventBusBuilder : IEventBusBuilder
{
    public IServiceCollection Services { get; }

    /// <summary>
    /// Used for logging in default retry policy
    /// default: true
    /// </summary>
    public bool EnableLog { get; set; } = true;

    public EventBusBuilder(IServiceCollection services) => Services = services;

    public IEventBusBuilder UseMiddleware(Type middleware, ServiceLifetime middlewareLifetime = ServiceLifetime.Transient)
    {
        if (!typeof(IMiddleware<>).IsGenericInterfaceAssignableFrom(middleware))
            throw new ArgumentException($"{middleware.Name} doesn't implement IMiddleware<>");

        var descriptor = new ServiceDescriptor(typeof(IMiddleware<>), middleware, middlewareLifetime);
        Services.TryAddEnumerable(descriptor);
        return this;
    }
}
