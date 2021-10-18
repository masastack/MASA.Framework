using MASA.Utils.Models.Config;
using Microsoft.Extensions.Options;

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
    public void TestAddMultDaprEventBusAsync()
    {
        var options = new DispatcherOptions(new ServiceCollection())
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies()
        };
        options.UseDaprEventBus<IntegrationEventLogService>()
               .UseDaprEventBus<IntegrationEventLogService>();
        var serviceProvider = options.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IIntegrationEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestAddDaprEventBusAndNullServicesAsync()
    {
        var options = new DispatcherOptions(null);
        var ex = Assert.ThrowsException<ArgumentNullException>(() => options.UseDaprEventBus<IntegrationEventLogService>());
        Assert.IsTrue(ex.Message == $"Value cannot be null. (Parameter '{nameof(options.Services)}')");
    }

    [TestMethod]
    public void TestAddDaprEventBusAndNullAssemblyAsync()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new DispatcherOptions(new ServiceCollection())
        {
            Assemblies = null
        });
    }

    [TestMethod]
    public async Task TestPublishAsync()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWork> unitWork = new();
        Mock<DbTransaction> dbTransaction = new();
        unitWork.Setup(u => u.Transaction).Returns(dbTransaction.Object);
        services.AddScoped((serviceProvider) => unitWork.Object);
        services.AddOptions();
        services.AddLogging();
        services.AddDaprEventBus<IntegrationEventLogService>(options =>
        {
        });
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        var serviceProvider = services.BuildServiceProvider();
        var integrationEventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await integrationEventBus.PublishAsync(@event);

        Assert.IsTrue(integrationEventBus.GetAllEventTypes().Count() == 3);
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

    [TestMethod]
    public async Task TestPublishEventAndNotUseEventBusAsync()
    {
        IOptions<DispatcherOptions> options = Options.Create(new DispatcherOptions(new ServiceCollection()));
        Mock<DaprClient> client = new();
        Mock<IIntegrationEventLogService> eventLogService = new();
        Mock<IOptionsMonitor<AppConfig>> appConfig = new();
        Mock<ILogger<IntegrationEventBus>> logger = new();

        var integrationEventBus = new IntegrationEventBus(options, client.Object, eventLogService.Object, appConfig.Object, logger.Object);
        var @event = new CreateUserEvent("tom");
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await integrationEventBus.PublishAsync(@event));
    }

    [TestMethod]
    public async Task TestPublishEventAsync()
    {
        IOptions<DispatcherOptions> options = Options.Create(new DispatcherOptions(new ServiceCollection())
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies()
        });
        Mock<DaprClient> client = new();
        Mock<IIntegrationEventLogService> eventLogService = new();
        Mock<IOptionsMonitor<AppConfig>> appConfig = new();
        Mock<ILogger<IntegrationEventBus>> logger = new();

        Mock<IEventBus> eventBus = new();
        eventBus.Setup(e => e.PublishAsync(It.IsAny<CreateUserEvent>())).Verifiable();
        eventBus.Setup(e => e.GetAllEventTypes()).Returns(() => new List<Type>()
        {
            typeof(CreateUserEvent),
            typeof(ForgetPasswordEvent),
            typeof(RegisterUserIntegrationEvent)
        });

        var integrationEventBus = new IntegrationEventBus(options, client.Object, eventLogService.Object, appConfig.Object, logger.Object, eventBus.Object);
        var @event = new CreateUserEvent("tom");
        await integrationEventBus.PublishAsync(@event);

        Assert.IsTrue(integrationEventBus.GetAllEventTypes().Count() == 3);
    }

    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return Task.CompletedTask;
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return Task.CompletedTask;
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            return await Task.FromResult(new List<IntegrationEventLog>());
        }

        public Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction)
        {
            return Task.CompletedTask;
        }
    }
}
