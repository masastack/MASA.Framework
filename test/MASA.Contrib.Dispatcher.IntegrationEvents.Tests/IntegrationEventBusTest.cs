namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class IntegrationEventBusTest : TestBase
{
    [TestMethod]
    public async Task TestPublishSuccessAsync()
    {
        var serviceProvider = CreateDefaultProvider("RegisterUser");
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestPublishAsync()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWork> unitWork = new();
        Mock<DbTransaction> dbTransaction = new();
        unitWork.Setup(u => u.Transaction).Returns(dbTransaction.Object);
        services.AddScoped((serviceProvider) => unitWork.Object);

        services.AddDaprEventBus<IntegrationEventLogService>(options =>
        {
            Dispatcher.Events.DispatcherOptionsExtensions.UseEventBus(options);
        });
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestPublishFailedAsync()
    {
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var serviceProvider = CreateCustomerDaprPubSubProvider(DAPR_PUBSUB_NAME, (services) =>
         {
             Mock<DaprClient> daprClient = new();
             daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, @event.Topic, It.IsAny<RegisterUserIntegrationEvent>(), default))
                 .Throws(new Exception("send failure"));
             services.AddSingleton(_ => daprClient.Object);
             services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
         });
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestDbTransactionPublishSuccessAsync()
    {
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var serviceProvider = CreateDefaultProvider(@event.Topic);
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestDbTransactionPublishFailedAsync()
    {
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var serviceProvider = CreateCustomerDaprPubSubProvider(DAPR_PUBSUB_NAME, (services) =>
        {
            Mock<DaprClient> daprClient = new();
            daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, @event.Topic, It.IsAny<RegisterUserIntegrationEvent>(), default))
                .Throws(new Exception("send failure"));
            services.AddSingleton(_ => daprClient.Object);
            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        });
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task CheckCustomerPubSubName()
    {
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var daprPubSubName = "PUBSUB";
        var serviceProvider = CreateCustomerDaprPubSubProvider(daprPubSubName, @event.Topic);
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task CheckPublishEvent()
    {
        ForgetPasswordEvent @event = new ForgetPasswordEvent()
        {
            Account = "lisa"
        };
        var daprPubSubName = "PUBSUB";
        var serviceProvider = CreateCustomerDaprPubSubProvider(daprPubSubName, "");
        var eventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await eventBus.PublishAsync(@event);
    }
}
