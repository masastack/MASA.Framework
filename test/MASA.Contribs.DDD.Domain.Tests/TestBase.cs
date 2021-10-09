using MASA.BuildingBlocks.Data.Uow;
using Moq;

namespace MASA.Contribs.DDD.Domain.Tests;

public class TestBase
{
    protected const string DAPR_PUBSUB_NAME = "pubsub";

    protected IServiceProvider CreateDefaultProvider()
    {
        return CreateProvider((services) =>
        {
            Mock<IIntegrationEventBus> integrationEventBus = new();
            services.AddScoped(typeof(IIntegrationEventBus), serviceProvider => integrationEventBus.Object);
        });
    }

    protected IServiceProvider CreateProvider(Action<IServiceCollection>? action = null)
    {
        var services = new ServiceCollection();
        action?.Invoke(services);
        services.AddDomainEventBus(options =>
        {
            Mock<IEventBus> eventBus = new();
            services.AddScoped(typeof(IEventBus), serviceProvider => eventBus.Object);

            Mock<IUnitOfWork> unitOfWork = new();
            services.AddScoped(typeof(IUnitOfWork), serviceProvider => unitOfWork.Object);
        });
        return services.BuildServiceProvider();
    }
}
