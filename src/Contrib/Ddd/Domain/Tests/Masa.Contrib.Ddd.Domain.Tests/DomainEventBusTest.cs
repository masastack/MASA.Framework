// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests;

[TestClass]
public class DomainEventBusTest
{
    private Mock<IEventBus> _eventBus;
    private Mock<IIntegrationEventBus> _integrationEventBus;
    private DomainEventBus _domainEventBus;

    private static readonly FieldInfo EventQueueFileInfo =
        typeof(DomainEventBus).GetField("_eventQueue", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [TestInitialize]
    public void Test()
    {
        _eventBus = new();
        _eventBus.Setup(bus => bus.PublishAsync(It.IsAny<IEvent>(), default)).Verifiable();
        _integrationEventBus = new();
        _integrationEventBus.Setup(bus => bus.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).Verifiable();
        Mock<IUnitOfWork> unitOfWork = new();
        _domainEventBus = new DomainEventBus(_eventBus.Object,
            _integrationEventBus.Object,
            unitOfWork.Object
        );
    }

    [TestMethod]
    public async Task TestPublishAsyncByEvent()
    {
        var registerUserEvent = new RegisterUserEvent();
        await _domainEventBus.PublishAsync(registerUserEvent);

        _integrationEventBus.Verify(bus => bus.PublishAsync(It.IsAny<IIntegrationEvent>(), default), Times.Never);
        _eventBus.Verify(bus => bus.PublishAsync(It.IsAny<IEvent>(), default), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishAsyncByIntegrationEvent()
    {
        var changeOrderStateIntegrationEvent = new ChangeOrderStateIntegrationEvent();
        await _domainEventBus.PublishAsync(changeOrderStateIntegrationEvent);

        _integrationEventBus.Verify(bus => bus.PublishAsync(It.IsAny<IIntegrationEvent>(), default), Times.Once);
        _eventBus.Verify(bus => bus.PublishAsync(It.IsAny<IEvent>(), default), Times.Never);
    }

    [TestMethod]
    public void TestAddDomainEventBusReturnDomainServiceIsNotNull()
    {
        var services = new ServiceCollection();
        Mock<IEventBus> eventBus = new();
        Mock<IIntegrationEventBus> integrationEventBus = new();
        Mock<IUnitOfWork> unitOfWork = new();
        services.AddScoped(_ => eventBus.Object);
        services.AddScoped(_ => integrationEventBus.Object);
        services.AddScoped(_ => unitOfWork.Object);
        services.AddDomainEventBus();
        var serviceProvider = services.BuildServiceProvider();
        var domainService = serviceProvider.GetService<UserDomainService>();
        Assert.IsNotNull(domainService);
        Assert.AreNotEqual(default, domainService.EventBus);
    }

    [TestMethod]
    public async Task TestEnqueueAsync()
    {
        Mock<IEventBus> eventBus = new();
        eventBus.Setup(bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>())).Verifiable();
        Mock<IIntegrationEventBus> integrationEventBus = new();
        Mock<IUnitOfWork> unitOfWork = new();
        var domainEventBus = new DomainEventBus(
            eventBus.Object,
            integrationEventBus.Object,
            unitOfWork.Object);
        var eventQueue = GetEventQueue(domainEventBus);
        Assert.AreEqual(0, eventQueue.Count);

        var registerUserDomainEvent = new RegisterUserDomainEvent()
        {
            Name = "masa"
        };
        await domainEventBus.Enqueue(registerUserDomainEvent);
        eventQueue = GetEventQueue(domainEventBus);
        Assert.AreEqual(1, eventQueue.Count);

        Assert.IsTrue(await domainEventBus.AnyQueueAsync());
        await domainEventBus.PublishQueueAsync();

        eventBus.Verify(bus => bus.PublishAsync(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public void TestRegisterDomainService()
    {
        var services = new ServiceCollection();
        Mock<IDomainEventBus> eventBus = new();
        services.AddScoped<IDomainEventBus>(_ => eventBus.Object);
        services.RegisterDomainService(new List<Type>()
        {
            typeof(UserDomainService)
        });
        var serviceProvider = services.BuildServiceProvider();
        var userDomainService = serviceProvider.GetService<UserDomainService>();
        Assert.IsNotNull(userDomainService);
        Assert.AreEqual(eventBus.Object, userDomainService.EventBus);
    }

    private static ConcurrentQueue<IDomainEvent> GetEventQueue(DomainEventBus domainEventBus)
    {
        var eventQueue = EventQueueFileInfo.GetValue(domainEventBus) as ConcurrentQueue<IDomainEvent>;
        Assert.IsNotNull(eventQueue);
        return eventQueue;
    }
}
