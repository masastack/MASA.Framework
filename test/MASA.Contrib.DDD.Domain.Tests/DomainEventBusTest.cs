namespace MASA.Contrib.DDD.Domain.Tests;

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
        var serviceProvider = CreateDefaultProvider();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        await eventBus.PublishAsync(@event);

        Assert.IsTrue(eventBus.GetAllEventTypes().Count() == 5);
    }

    [TestMethod]
    public async Task TestPublishIntegrationDomainEventAsync()
    {
        PaymentFailedIntegrationDomainEvent @event = new PaymentFailedIntegrationDomainEvent()
        {
            OrderId = new Random().Next(10000, 1000000).ToString()
        };
        var serviceProvider = CreateDefaultProvider();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestPublishDomainCommandAsync()
    {
        var @command = new CreateProductDomainCommand()
        {
            Name = "Phone"
        };

        var serviceProvider = CreateDefaultProvider();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        await eventBus.PublishAsync(@command);
    }

    [TestMethod]
    public async Task TestAddMultDomainEventBusAsync()
    {
        var services = new ServiceCollection();

        services.AddDomainEventBus(options =>
        {
            options.Assemblies = new System.Reflection.Assembly[1] { typeof(TestBase).Assembly };
            Mock<IEventBus> eventBus = new();
            eventBus.Setup(e => e.PublishAsync(It.IsAny<IEvent>())).Verifiable();
            services.AddScoped(typeof(IEventBus), serviceProvider => eventBus.Object);

            Mock<IUnitOfWork> unitOfWork = new();
            services.AddScoped(typeof(IUnitOfWork), serviceProvider => unitOfWork.Object);

            Mock<IIntegrationEventBus> integrationEventBus = new();
            integrationEventBus.Setup(e => e.PublishAsync(It.IsAny<IIntegrationEvent>())).Verifiable();
            services.AddScoped(typeof(IIntegrationEventBus), serviceProvider => integrationEventBus.Object);
        }).AddDomainEventBus();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IDomainEventBus>().Count() == 1);

        var userDomainService = serviceProvider.GetService<UserDomainService>();
        Assert.IsNotNull(userDomainService);

        Assert.IsTrue(await userDomainService.RegisterUserSucceededAsync("tom") == "succeed");
    }

    [TestMethod]
    public void TestNotUseEventBus()
    {
        var services = new ServiceCollection();

        var ex = Assert.ThrowsException<Exception>(() => services.AddDomainEventBus());
        Assert.IsTrue(ex.Message == "Please add EventBus first.");
    }

    [TestMethod]
    public void TestNotUseUnitOfWork()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(serviceProvider => eventBus.Object);

        var ex = Assert.ThrowsException<Exception>(() => services.AddDomainEventBus());
        Assert.IsTrue(ex.Message == "Please add UoW first.");
    }

    [TestMethod]
    public void TestNotUseIntegrationEventBus()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(serviceProvider => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(serviceProvider => uoW.Object);

        var ex = Assert.ThrowsException<Exception>(() => services.AddDomainEventBus());
        Assert.IsTrue(ex.Message == "Please add IntegrationEventBus first.");
    }

    [TestMethod]
    public void TestNullAssembly()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(serviceProvider => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(serviceProvider => uoW.Object);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        services.AddScoped(serviceProvider => integrationEventBus.Object);

        Assert.ThrowsException<ArgumentNullException>(() => services.AddDomainEventBus(options => { options.Assemblies = null; }));
    }

    [TestMethod]
    public void TestNotRepository()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(serviceProvider => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(serviceProvider => uoW.Object);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        services.AddScoped(serviceProvider => integrationEventBus.Object);

        Assert.ThrowsException<NotImplementedException>(() =>
        {
            services.AddDomainEventBus(options =>
            {
                options.Assemblies = new System.Reflection.Assembly[1] { typeof(User).Assembly };
            });
        });
    }


    [TestMethod]
    public void TestUserRepository()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(serviceProvider => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(serviceProvider => uoW.Object);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        services.AddScoped(serviceProvider => integrationEventBus.Object);
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddDomainEventBus(options =>
        {
            options.Assemblies = new System.Reflection.Assembly[2] { typeof(User).Assembly, typeof(UserRepository).Assembly };
        });
    }

    [TestMethod]
    public async Task TestPublishQueueAsync()
    {
        var services = new ServiceCollection();

        //todo: Temporary results, used to show the enqueue and dequeue order
        int result = 0;

        Mock<IEventBus> eventBus = new();
        eventBus
            .Setup(e => e.PublishAsync(It.IsAny<IEvent>()))
            .Callback<IEvent>(async cmd =>
            {
                if (result == 0)
                {
                    result = 3;
                }
                else
                {
                    result = 4;
                }
                await Task.FromResult(result);
            });
        Mock<IIntegrationEventBus> integrationEventBus = new();
        integrationEventBus
            .Setup(e => e.PublishAsync(It.IsAny<IIntegrationEvent>()))
            .Callback<IIntegrationEvent>(async cmd =>
            {
                if (result == 3)
                {
                    result = 1;
                }
                else
                {
                    result = 2;
                }
                await Task.FromResult(result);
            });

        var uoW = new Mock<IUnitOfWork>();
        uoW.Setup(u => u.CommitAsync(default)).Verifiable();

        var options = Options.Create(new DispatcherOptions(services) { Assemblies = AppDomain.CurrentDomain.GetAssemblies() });

        var domainEventBus = new DomainEventBus(eventBus.Object, integrationEventBus.Object, uoW.Object, options);

        // todo: It has no practical meaning, just to show the order of entering and leaving the team
        await domainEventBus.Enqueue(new PaymentSucceededDomainEvent() { OrderId = "ef5f84db-76e4-4c79-9815-99a1543b6589" });
        await domainEventBus.Enqueue(new PaymentFailedIntegrationDomainEvent() { OrderId = "d65c1a0c-6e44-40ce-9737-738fa1dcdab4" });
        await domainEventBus.PublishQueueAsync();
        Assert.IsTrue(result == 1);
    }

    [TestMethod]
    public async Task TestPublishDomainQuery()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        eventBus.Setup(e => e.PublishAsync(It.IsAny<ProductItemDomainQuery>()))
            .Callback<ProductItemDomainQuery>(async query =>
            {
                query.Result = "apple";
            });
        var integrationEventBus = new Mock<IIntegrationEventBus>();
        var uoW = new Mock<IUnitOfWork>();
        uoW.Setup(u => u.CommitAsync(default)).Verifiable();

        var options = Options.Create(new DispatcherOptions(services) { Assemblies = AppDomain.CurrentDomain.GetAssemblies() });

        var domainEventBus = new DomainEventBus(eventBus.Object, integrationEventBus.Object, uoW.Object, options);

        var query = new ProductItemDomainQuery() { ProductId = "2f8d4c3c-1736-4e56-a188-f865da6a63d1" };
        await domainEventBus.PublishAsync(query);
        Assert.IsTrue(query.Result == "apple");
    }
}
