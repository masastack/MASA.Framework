namespace Masa.Contrib.Ddd.Domain.Tests;

[TestClass]
public class DomainEventBusTest
{
    private Assembly[] _defaultAssemblies = default!;
    private IServiceCollection _services = default!;
    private Mock<IEventBus> _eventBus = default!;
    private Mock<IIntegrationEventBus> _integrationEventBus = default!;
    private Mock<IUnitOfWork> _uoW = default!;
    private IOptions<Options.DispatcherOptions> _dispatcherOptions = default!;

    [TestInitialize]
    public void Initialize()
    {
        _defaultAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        _services = new ServiceCollection();
        _eventBus = new();
        _integrationEventBus = new();
        _uoW = new();
        _dispatcherOptions = Microsoft.Extensions.Options.Options.Create(new Options.DispatcherOptions(new ServiceCollection(), _defaultAssemblies));
    }

    [TestMethod]
    public void TestGetAllEventTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var eventTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type));
        _eventBus.Setup(eventBus => eventBus.GetAllEventTypes()).Returns(() => eventTypes);
        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, _dispatcherOptions);

        Assert.IsTrue(domainEventBus.GetAllEventTypes().Count() == eventTypes.Count(), "");
    }

    [TestMethod]
    public async Task TestPublishDomainEventAsync()
    {
        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, _dispatcherOptions);
        _eventBus.Setup(eventBus => eventBus.PublishAsync(It.IsAny<PaymentSucceededDomainEvent>())).Verifiable();

        var domainEvent = new PaymentSucceededDomainEvent(new Random().Next(10000, 1000000).ToString());
        await domainEventBus.PublishAsync(domainEvent);

        _eventBus.Verify(eventBus => eventBus.PublishAsync(domainEvent), Times.Once, "PublishAsync is executed multiple times");
        _integrationEventBus.Verify(integrationEventBus => integrationEventBus.PublishAsync(domainEvent), Times.Never, "integrationEventBus should not be executed");
        Assert.IsTrue(domainEvent.UnitOfWork!.Equals(_uoW.Object));
    }

    [TestMethod]
    public async Task TestPublishIntegrationDomainEventAsync()
    {
        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, _dispatcherOptions);
        _integrationEventBus.Setup(integrationEventBus => integrationEventBus.PublishAsync(It.IsAny<IIntegrationEvent>())).Verifiable();
        var integrationDomainEvent = new PaymentFailedIntegrationDomainEvent()
        {
            OrderId = new Random().Next(10000, 1000000).ToString()
        };
        await domainEventBus.PublishAsync(integrationDomainEvent);

        _eventBus.Verify(eventBus => eventBus.PublishAsync(It.IsAny<IEvent>()), Times.Never, "eventBus should not be executed");
        _integrationEventBus.Verify(integrationEventBus => integrationEventBus.PublishAsync(It.IsAny<IIntegrationEvent>()), Times.Once, " PublishAsync is executed multiple times");
    }

    [TestMethod]
    public async Task TestPublishDomainCommandAsync()
    {
        _uoW.Setup(u => u.CommitAsync(default)).Verifiable();
        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, _dispatcherOptions);
        _eventBus.Setup(eventBus => eventBus.PublishAsync(It.IsAny<CreateProductDomainCommand>()))
            .Callback<CreateProductDomainCommand>((domainEvent) =>
            {
                Mock<IRepository<Users>> userRepository = new();
                var user = new Users()
                {
                    Name = "Jim"
                };
                userRepository.Setup(repository => repository.AddAsync(It.IsAny<Users>(), CancellationToken.None)).Verifiable();
                domainEvent.UnitOfWork!.CommitAsync();
            });

        var @command = new CreateProductDomainCommand()
        {
            Name = "Phone"
        };
        await domainEventBus.PublishAsync(@command);

        _eventBus.Verify(eventBus => eventBus.PublishAsync(@command), Times.Once, "PublishAsync is executed multiple times");
        _uoW.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [TestMethod]
    public void TestAddMultDomainEventBusAsync()
    {
        _services.AddScoped(_ => _eventBus.Object);
        _services.AddScoped(_ => _integrationEventBus.Object);
        _services.AddScoped(_ => _uoW.Object);

        _services.AddDomainEventBus(new[] { typeof(DomainEventBusTest).Assembly }).AddDomainEventBus();
        var serviceProvider = _services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IDomainEventBus>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IOptions<Options.DispatcherOptions>>().Count() == 1);
    }

    [TestMethod]
    public void TestNotUseEventBus()
    {
        var ex = Assert.ThrowsException<Exception>(()
            => _services.AddDomainEventBus()
        );
        Assert.IsTrue(ex.Message == "Please add EventBus first.");
    }

    [TestMethod]
    public void TestNotUseUnitOfWork()
    {
        var eventBus = new Mock<IEventBus>();
        _services.AddScoped(_ => eventBus.Object);

        var ex = Assert.ThrowsException<Exception>(() =>
            _services.AddDomainEventBus(new Assembly[1] { typeof(DomainEventBusTest).Assembly }));
        Assert.IsTrue(ex.Message == "Please add UoW first.");
    }

    [TestMethod]
    public void TestNotUseIntegrationEventBus()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(_ => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(_ => uoW.Object);

        var ex = Assert.ThrowsException<Exception>(()
            => services.AddDomainEventBus(new Assembly[1] { typeof(DomainEventBusTest).Assembly })
        );
        Assert.IsTrue(ex.Message == "Please add IntegrationEventBus first.");
    }

    [TestMethod]
    public void TestNotRepository()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(_ => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(_ => uoW.Object);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        services.AddScoped(_ => integrationEventBus.Object);

        Assert.ThrowsException<NotImplementedException>(() =>
        {
            services.AddDomainEventBus(new Assembly[1] { typeof(Users).Assembly });
        });
    }

    [TestMethod]
    public void TestUserRepository()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        services.AddScoped(_ => eventBus.Object);

        var uoW = new Mock<IUnitOfWork>();
        services.AddScoped(_ => uoW.Object);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        services.AddScoped(_ => integrationEventBus.Object);

        Mock<IRepository<Users>> repository = new();
        services.AddScoped(_ => repository.Object);
        services.AddDomainEventBus(new[] { typeof(Users).Assembly, typeof(DomainEventBusTest).Assembly });
    }

    [TestMethod]
    public async Task TestPublishQueueAsync()
    {
        var domainEvent = new PaymentSucceededDomainEvent("ef5f84db-76e4-4c79-9815-99a1543b6589");
        var integrationDomainEvent = new PaymentFailedIntegrationDomainEvent { OrderId = "d65c1a0c-6e44-40ce-9737-738fa1dcdab4" };

        _eventBus
            .Setup(eventBus => eventBus.PublishAsync(It.IsAny<IDomainEvent>()))
            .Callback(() =>
           {
               _integrationEventBus.Verify(integrationEventBus => integrationEventBus.PublishAsync(integrationDomainEvent), Times.Never, "Sent in the wrong order");
           });

        _integrationEventBus
            .Setup(integrationEventBus => integrationEventBus.PublishAsync(It.IsAny<IDomainEvent>()))
            .Callback(() =>
            {
                _eventBus.Verify(eventBus => eventBus.PublishAsync((IDomainEvent)domainEvent), Times.Once, "Sent in the wrong order");
            });

        var uoW = new Mock<IUnitOfWork>();
        uoW.Setup(u => u.CommitAsync(default)).Verifiable();

        var options = Microsoft.Extensions.Options.Options.Create(new Options.DispatcherOptions(_services, AppDomain.CurrentDomain.GetAssemblies()));

        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, uoW.Object, options);

        await domainEventBus.Enqueue(domainEvent);
        await domainEventBus.Enqueue(integrationDomainEvent);

        await domainEventBus.PublishQueueAsync();

        _eventBus.Verify(eventBus => eventBus.PublishAsync((IDomainEvent)domainEvent), Times.Once, "Sent in the wrong order");
        _integrationEventBus.Verify(integrationEventBus => integrationEventBus.PublishAsync(integrationDomainEvent), Times.Never, "Sent in the wrong order");
    }

    [TestMethod]
    public async Task TestPublishDomainQueryAsync()
    {
        var services = new ServiceCollection();

        var eventBus = new Mock<IEventBus>();
        eventBus.Setup(e => e.PublishAsync(It.IsAny<ProductItemDomainQuery>()))
            .Callback<ProductItemDomainQuery>(query =>
            {
                if (query.ProductId == "2f8d4c3c-1736-4e56-a188-f865da6a63d1")
                    query.Result = "apple";
            });
        var integrationEventBus = new Mock<IIntegrationEventBus>();
        var uoW = new Mock<IUnitOfWork>();
        uoW.Setup(u => u.CommitAsync(default)).Verifiable();

        var options = Microsoft.Extensions.Options.Options.Create(new Options.DispatcherOptions(services, AppDomain.CurrentDomain.GetAssemblies()));

        var domainEventBus = new DomainEventBus(eventBus.Object, integrationEventBus.Object, uoW.Object, options);

        var query = new ProductItemDomainQuery() { ProductId = "2f8d4c3c-1736-4e56-a188-f865da6a63d1" };

        await domainEventBus.PublishAsync(query);
        Assert.IsTrue(query.Result == "apple");
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        var services = new ServiceCollection();

        _uoW.Setup(uow => uow.CommitAsync(CancellationToken.None)).Verifiable();
        Mock<IOptions<Options.DispatcherOptions>> options = new();

        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, options.Object);
        await domainEventBus.CommitAsync(CancellationToken.None);

        _uoW.Verify(u => u.CommitAsync(default), Times.Once, "CommitAsync must be called only once");
    }

    [TestMethod]
    public void TestParameterInitialization()
    {
        var id = Guid.NewGuid();
        var createTime = DateTime.UtcNow;

        var domainCommand = new DomainCommand();
        Assert.IsTrue(domainCommand.Id != default);
        Assert.IsTrue(domainCommand.CreationTime != default && domainCommand.CreationTime >= createTime);

        domainCommand = new DomainCommand(id, createTime);
        Assert.IsTrue(domainCommand.Id == id);
        Assert.IsTrue(domainCommand.CreationTime == createTime);

        var domainEvent = new DomainEvent();
        Assert.IsTrue(domainEvent.Id != default);
        Assert.IsTrue(domainEvent.CreationTime != default && domainEvent.CreationTime >= createTime);

        domainEvent = new DomainEvent(id, createTime);
        Assert.IsTrue(domainEvent.Id == id);
        Assert.IsTrue(domainEvent.CreationTime == createTime);

        var domainQuery = new ProductItemDomainQuery()
        {
            ProductId = Guid.NewGuid().ToString()
        };
        Assert.IsTrue(domainQuery.Id != default);
        Assert.IsTrue(domainQuery.CreationTime != default && domainQuery.CreationTime >= createTime);
    }

    [TestMethod]
    public void TestDomainQueryUnitOfWork()
    {
        var domainQuery = new ProductItemDomainQuery()
        {
            ProductId = Guid.NewGuid().ToString()
        };
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            domainQuery.UnitOfWork = _uoW.Object;
        });
        Assert.IsNull(domainQuery.UnitOfWork);
    }

    [TestMethod]
    public async Task TestDomainServiceAsync()
    {
        _integrationEventBus.Setup(integrationEventBus => integrationEventBus.PublishAsync(It.IsAny<IIntegrationEvent>())).Verifiable();

        _services.AddDomainEventBus(new[] { typeof(DomainEventBusTest).Assembly },
            options =>
            {
                options.Services.AddScoped(_ => _eventBus.Object);
                options.Services.AddScoped(_ => _integrationEventBus.Object);
                options.Services.AddScoped(_ => _uoW.Object);
            });
        var serviceProvider = _services.BuildServiceProvider();

        var userDomainService = serviceProvider.GetRequiredService<UserDomainService>();
        var domainIntegrationEvent = new RegisterUserSucceededDomainIntegrationEvent() { Account = "Tom" };
        await userDomainService.RegisterUserSucceededAsync(domainIntegrationEvent);

        _integrationEventBus.Verify(integrationEventBus => integrationEventBus.PublishAsync(It.IsAny<IIntegrationEvent>()), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishEvent()
    {
        var domainEventBus = new DomainEventBus(_eventBus.Object, _integrationEventBus.Object, _uoW.Object, _dispatcherOptions);
        _eventBus.Setup(eventBus => eventBus.PublishAsync(It.IsAny<ForgetPasswordEvent>())).Verifiable();

        var @event = new ForgetPasswordEvent()
        {
            Account = "Tom"
        };
        await domainEventBus.PublishAsync(@event);
        _eventBus.Verify(eventBus => eventBus.PublishAsync(@event), Times.Once);
    }
}
