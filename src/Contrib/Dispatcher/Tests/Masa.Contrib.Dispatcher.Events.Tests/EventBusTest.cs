// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class EventBusTest
{
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly Mock<ILocalEventBusWrapper> _localEventBusWrapperMock;
    private readonly Mock<IIntegrationEventBus> _integrationEventBusMock;
    private readonly Mock<IExecuteProvider> _executeProvider;
    private readonly EventBus _eventBus;

    public EventBusTest()
    {
        _serviceProvider = new();
        _localEventBusWrapperMock = new();
        _integrationEventBusMock = new();
        _executeProvider = new Mock<IExecuteProvider>();

        _serviceProvider.Setup(provider => provider.GetService(typeof(ILocalEventBusWrapper))).Returns(_localEventBusWrapperMock.Object);
        _serviceProvider.Setup(provider => provider.GetService(typeof(IIntegrationEventBus))).Returns(_integrationEventBusMock.Object);
        _serviceProvider.Setup(provider => provider.GetService(typeof(IExecuteProvider))).Returns(_executeProvider.Object);

        _eventBus = new EventBus(_serviceProvider.Object);
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventAsync()
    {
        var integrationEvent = new OrderPaymentFailedIntegrationEvent()
        {
            OrderId = Guid.NewGuid().ToString(),
        };
        await _eventBus.PublishAsync(integrationEvent);
        _localEventBusWrapperMock.Verify(bus => bus.PublishAsync(integrationEvent, It.IsAny<IEnumerable<IEventMiddleware<OrderPaymentFailedIntegrationEvent>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _integrationEventBusMock.Verify(bus => bus.PublishAsync(integrationEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventByIntegrationEventBusIsNullAsync()
    {
        _serviceProvider.Setup(provider => provider.GetService(typeof(IEnumerable<IEventMiddleware<AddGoodsEvent>>)))
            .Returns(() => new List<IEventMiddleware<AddGoodsEvent>>());
        _executeProvider.Setup(provider => provider.ExecuteHandler()).Verifiable();
        _localEventBusWrapperMock.Setup(eventBus => eventBus.PublishAsync(It.IsAny<AddGoodsEvent>(), It.IsAny<IEnumerable<IEventMiddleware<AddGoodsEvent>>>(), It.IsAny<CancellationToken>()));

        var @event = new AddGoodsEvent
        {
            Name = "Water Purifier",
            Stock = 1
        };
        await _eventBus.PublishAsync(@event);

        _localEventBusWrapperMock.Verify(eventBus => eventBus.PublishAsync(@event, It.IsAny<IEnumerable<IEventMiddleware<AddGoodsEvent>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
