namespace MASA.Contrib.DDD.Domain.Tests;

public class TestBase
{
    protected const string DAPR_PUBSUB_NAME = "pubsub";

    protected IServiceProvider CreateDefaultProvider()
    {
        return CreateProvider((services) =>
        {
            Mock<IIntegrationEventBus> integrationEventBus = new();
            integrationEventBus.Setup(e => e.PublishAsync(It.IsAny<IIntegrationEvent>())).Verifiable();
            services.AddScoped(typeof(IIntegrationEventBus), serviceProvider => integrationEventBus.Object);
        });
    }

    protected IServiceProvider CreateProvider(Action<IServiceCollection>? action = null)
    {
        var services = new ServiceCollection();
        action?.Invoke(services);
        services.AddDomainEventBus(options =>
        {
            options.Assemblies = new System.Reflection.Assembly[1] { typeof(TestBase).Assembly };
            Mock<IEventBus> eventBus = new();
            eventBus.Setup(e => e.PublishAsync(It.IsAny<IEvent>())).Verifiable();
            services.AddScoped(typeof(IEventBus), serviceProvider => eventBus.Object);

            Mock<IUnitOfWork> unitOfWork = new();
            services.AddScoped(typeof(IUnitOfWork), serviceProvider => unitOfWork.Object);
        });
        return services.BuildServiceProvider();
    }
}
