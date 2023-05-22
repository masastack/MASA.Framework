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
        eventBuilder.UseMiddleware(typeof(LoggingEventMiddleware<>));

        Assert.AreEqual(1, services.Count);
    }

    [TestMethod]
    public void TestUseMiddleware2()
    {
        var services = new ServiceCollection();
        var eventBuilder = new EventBusBuilder(services);
        eventBuilder.UseMiddleware(new[] { typeof(LoggingEventMiddleware<>), typeof(RequestEventMiddleware<>) });

        Assert.AreEqual(2, services.Count);
    }

    [TestMethod]
    public async Task TestMiddlewareByOrderOfExecutionAsync()
    {
        var services = new ServiceCollection();
        services.AddEventBus(new[] { typeof(CustomEventMiddleware<>).Assembly }, eventBusBuilder
            => eventBusBuilder.UseMiddleware(new[] { typeof(CustomEventMiddleware<>), typeof(Custom2EventMiddleware<>) }));
        var serviceProvider = services.BuildServiceProvider();

        var middlewares = serviceProvider.GetService<IEnumerable<IEventMiddleware<MiddlewareEvent>>>();
        Assert.IsNotNull(middlewares);
        Assert.AreEqual(4, middlewares.Count());

        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var @event = new MiddlewareEvent();
        await eventBus.PublishAsync(@event);

        Assert.AreEqual(3, @event.Results.Count);
        Assert.AreEqual(nameof(CustomEventMiddleware<MiddlewareEvent>), @event.Results[0]);
        Assert.AreEqual(nameof(Custom2EventMiddleware<MiddlewareEvent>), @event.Results[1]);
        Assert.AreEqual(nameof(EventHandlers.MiddlewareEventHandler), @event.Results[2]);
    }

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
}
