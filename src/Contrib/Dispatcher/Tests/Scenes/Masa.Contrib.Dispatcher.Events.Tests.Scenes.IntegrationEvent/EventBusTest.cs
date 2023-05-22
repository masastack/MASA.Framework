// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.Scenes.IntegrationEvent;

[TestClass]
public class EventBusTest
{
    // [TestMethod]
    // public async Task TestPublishIntegrationEventAsync()
    // {
    //     Mock<ILocalEventBus> localEventBus = new();
    //     Mock<IIntegrationEventBus> integrationEventBus = new();
    //     Mock<IExecuteProvider> executeProvider = new();
    //
    //     Mock<IServiceProvider> serviceProvider = new();
    //     serviceProvider.Setup(provider => provider.GetRequiredService<ILocalEventBus>()).Returns(localEventBus.Object);
    //     serviceProvider.Setup(provider => provider.GetRequiredService<IIntegrationEventBus>()).Returns(integrationEventBus.Object);
    //     serviceProvider.Setup(provider => provider.GetRequiredService<IExecuteProvider>()).Returns(executeProvider.Object);
    //
    //     var eventBus = new EventBus(serviceProvider.Object);
    //     var @event = new RegisterUserIntegrationEvent()
    //     {
    //         Account = "masa"
    //     };
    //     await eventBus.PublishAsync(@event);
    //     localEventBus.Verify(bus => bus.PublishAsync(@event, It.IsAny<CancellationToken>()), Times.Never);
    //     integrationEventBus.Verify(bus => bus.PublishAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
    // }
    //
    // [TestMethod]
    // public async Task TestPublishIntegrationEventByIntegrationEventBusIsNullAsync()
    // {
    //     Mock<ILocalEventBus> localEventBus = new();
    //     var eventBus = new EventBus(
    //         new Lazy<ILocalEventBus>(() => localEventBus.Object),
    //         new Lazy<IIntegrationEventBus?>(() => null));
    //     var @event = new RegisterUserIntegrationEvent()
    //     {
    //         Account = "masa"
    //     };
    //     await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await eventBus.PublishAsync(@event));
    // }
}
