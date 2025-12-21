// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class EventBusBuilderTest
{
    private readonly EventBusBuilder _eventBusBuilder;

    public EventBusBuilderTest()
    {
        var services = new ServiceCollection();
        _eventBusBuilder = new EventBusBuilder(services);
    }

    [TestMethod]
    public void TestUseMiddlewareBySingle()
    {
        _eventBusBuilder.UseMiddleware(typeof(LoggingEventMiddleware<>));

        Assert.AreEqual(1, _eventBusBuilder.Services.Count);
    }

    [TestMethod]
    public void TestUseMiddlewareByRepeat()
    {
        _eventBusBuilder.UseMiddleware(new[]
        {
            typeof(LoggingEventMiddleware<>), typeof(LoggingEventMiddleware<>)
        });

        Assert.AreEqual(1, _eventBusBuilder.Services.Count);
    }

    [TestMethod]
    public void TestUseMiddlewareByCollection()
    {
        _eventBusBuilder.UseMiddleware(new[]
        {
            typeof(LoggingEventMiddleware<>), typeof(RequestEventMiddleware<>)
        });

        Assert.AreEqual(2, _eventBusBuilder.Services.Count);
    }

    [TestMethod]
    public void TestUseMiddlewareByErrorMiddleware()
    {
        Assert.ThrowsExactly<ArgumentException>(() => _eventBusBuilder.UseMiddleware(typeof(EventBusBuilderTest)));
    }
}
