namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class RegisterServicesBusTest : TestBase
{
    [TestMethod]
    public void TestEmptyDaprPubSubName()
    {
        var daprPubSubName = string.Empty;
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var serviceProvider = CreateCustomerDaprPubSubProvider(daprPubSubName, "topic");
        });
    }

    [TestMethod]
    public void TestNotImplementedEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

        Mock<DaprClient> daprClient = new();
        daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, "topic", It.IsAny<IIntegrationEvent>(), default)).Verifiable();
        services.AddSingleton(_ => daprClient.Object);

        Assert.ThrowsException<NotImplementedException>(() =>
        {
            services.AddDaprEventBus<IntegrationEventLogService>();
        });
    }

    [TestMethod]
    public void TestNotImplementedUow()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

        Mock<DaprClient> daprClient = new();
        daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, "topic", It.IsAny<IIntegrationEvent>(), default)).Verifiable();
        services.AddSingleton(_ => daprClient.Object);

        Mock<IEventBus> eventBus = new();
        eventBus.Setup(e => e.PublishAsync(It.IsAny<IEvent>())).Verifiable();
        services.AddScoped(typeof(IEventBus), eventBus.Object.GetType());

        Assert.ThrowsException<NotImplementedException>(() =>
        {
            services.AddDaprEventBus<IntegrationEventLogService>(options => options.PubSubName = DAPR_PUBSUB_NAME);
        });
    }

    //[TestMethod]
    //public void CheckCustomerPubSubName()
    //{
    //    var serviceProvider = base.CreateProvider(DAPR_PUBSUB_NAME, services =>
    //     {
    //         Mock<DaprClient> daprClient = new();
    //         daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, "topic", It.IsAny<IIntegrationEvent>(), default)).Verifiable();
    //         services.AddSingleton(_ => daprClient.Object);
    //     });
    //}
}
