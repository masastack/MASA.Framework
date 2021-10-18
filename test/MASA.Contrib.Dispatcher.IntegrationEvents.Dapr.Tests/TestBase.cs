namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class TestBase
{
    protected const string DAPR_PUBSUB_NAME = "pubsub";

    protected IServiceProvider CreateDefaultProvider(string topic, Action<IServiceCollection>? action = null)
    {
        return CreateProvider(services =>
        {
            action?.Invoke(services);
            Mock<DaprClient> daprClient = new();
            daprClient.Setup(e => e.PublishEventAsync(DAPR_PUBSUB_NAME, topic, It.IsAny<IIntegrationEvent>(), default)).Verifiable();
            services.AddSingleton(_ => daprClient.Object);

            services.AddDaprEventBus<IntegrationEventLogService>();
        });
    }

    protected IServiceProvider CreateCustomerDaprPubSubProvider(string daprPubSubName, string topic, Action<IServiceCollection>? action = null)
    {
        return CreateCustomerDaprPubSubProvider(daprPubSubName, (services) =>
        {
            action?.Invoke(services);
            Mock<DaprClient> daprClient = new();
            daprClient.Setup(e => e.PublishEventAsync(daprPubSubName, topic, It.IsAny<IIntegrationEvent>(), default)).Verifiable();
            services.AddSingleton(_ => daprClient.Object);
        });
    }

    protected IServiceProvider CreateCustomerDaprPubSubProvider(string daprPubSubName, Action<IServiceCollection>? action = null)
    {
        return CreateProvider(services =>
        {
            action?.Invoke(services);
            services.AddDaprEventBus<IntegrationEventLogService>(options => options.PubSubName = daprPubSubName);
        });
    }

    private IServiceProvider CreateProvider(Action<IServiceCollection>? action = null)
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

        Mock<IEventBus> eventBus = new();
        eventBus.Setup(e => e.PublishAsync(It.IsAny<IEvent>())).Verifiable();
        services.AddScoped((serviceProvider) => eventBus.Object);

        Mock<IUnitOfWork> unitWork = new();
        Mock<DbTransaction> dbTransaction = new();
        unitWork.Setup(u => u.Transaction).Returns(dbTransaction.Object);
        services.AddScoped((serviceProvider) => unitWork.Object);

        action?.Invoke(services);
        return services.BuildServiceProvider();
    }
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

    public Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        return Task.FromResult(new List<IntegrationEventLog>().AsEnumerable());
    }

    public Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction)
    {
        return Task.CompletedTask;
    }
}
