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
