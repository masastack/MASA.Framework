// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests;

[TestClass]
public class DomainEventBusTest
{
    private Mock<IEventBus> _eventBus;
    private Mock<IIntegrationEventBus> _integrationEventBus;
    private DomainEventBus _domainEventBus;

    [TestInitialize]
    public void Test()
    {
        _eventBus = new();
        _eventBus.Setup(bus => bus.PublishAsync(It.IsAny<IEvent>(), default)).Verifiable();
        _integrationEventBus = new();
        _integrationEventBus.Setup(bus => bus.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).Verifiable();
        Mock<IUnitOfWork> unitOfWork = new();
        IOptions<DispatcherOptions> options =
            Microsoft.Extensions.Options.Options.Create(
                new DispatcherOptions(
                    new ServiceCollection(),
                    AppDomain.CurrentDomain.GetAssemblies()));
        _domainEventBus = new DomainEventBus(_eventBus.Object,
            _integrationEventBus.Object,
            unitOfWork.Object,
            options
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
}
