// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class EventBusBuilderTest
{
    [TestMethod]
    public void TestUseMiddleware()
    {
        var services = new ServiceCollection();
        var eventBuilder = new EventBusBuilder(services);
        eventBuilder.UseMiddleware(typeof(LoggingMiddleware<>));

        Assert.AreEqual(1, services.Count);
    }

    [TestMethod]
    public void TestUseMiddleware2()
    {
        var services = new ServiceCollection();
        var eventBuilder = new EventBusBuilder(services);
        eventBuilder.UseMiddleware(new[] { typeof(LoggingMiddleware<>), typeof(RequestMiddleware<>) });

        Assert.AreEqual(2, services.Count);
    }

    [TestMethod]
    public async Task TestMiddlewareByOrderOfExecutionAsync()
    {
        var services = new ServiceCollection();
        services.AddEventBus(new[] { typeof(CustomMiddleware<>).Assembly }, eventBusBuilder
            => eventBusBuilder.UseMiddleware(new[] { typeof(CustomMiddleware<>), typeof(Custom2Middleware<>) }));
        var serviceProvider = services.BuildServiceProvider();

        var middlewares = serviceProvider.GetService<IEnumerable<IMiddleware<MiddlewareEvent>>>();
        Assert.IsNotNull(middlewares);
        Assert.AreEqual(3, middlewares.Count());

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var @event = new MiddlewareEvent();
        await eventBus.PublishAsync(@event);

        Assert.AreEqual(3, @event.Results.Count);
        Assert.AreEqual(nameof(CustomMiddleware<MiddlewareEvent>), @event.Results[0]);
        Assert.AreEqual(nameof(Custom2Middleware<MiddlewareEvent>), @event.Results[1]);
        Assert.AreEqual(nameof(EventHandlers.MiddlewareEventHandler), @event.Results[2]);
    }

    public class RequestMiddleware<TEvent> : Middleware<TEvent> where TEvent : IEvent
    {
        private readonly ILogger<RequestMiddleware<TEvent>>? _logger;
        public RequestMiddleware(ILogger<RequestMiddleware<TEvent>>? logger = null) => _logger = logger;

        public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
        {
            var eventType = @event.GetType();
            _logger?.LogInformation("----- Handling command {FullName} ({event})", eventType.FullName, @event);
            await next();
        }
    }
}
