namespace MASA.Contribs.DDD.Domain.Tests;

[TestClass]
public class DomainEventBusTest : TestBase
{
    [TestMethod]
    public async Task TestPublishDomainEventAsync()
    {
        PaymentSucceededDomainEvent @event = new PaymentSucceededDomainEvent()
        {
            OrderId = new Random().Next(10000, 1000000).ToString()
        };
        var serviceProvider = base.CreateDefaultProvider();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestPublishIntegrationDomainEventAsync()
    {
        PaymentFailedIntegrationDomainEvent @event = new PaymentFailedIntegrationDomainEvent()
        {
            OrderId = new Random().Next(10000, 1000000).ToString()
        };
        var serviceProvider = base.CreateDefaultProvider();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        await eventBus.PublishAsync(@event);
    }
}
