// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class IntegrationEventBusTest
{
    private Mock<IDispatcherOptions> _options;
    private Mock<IOptions<IntegrationEventOptions>> _dispatcherOptions;
    private Mock<IPublisher> _publisher;
    private Mock<ILogger<IntegrationEventBus>> _logger;
    private Mock<IIntegrationEventLogService> _eventLog;
    private Mock<IOptionsMonitor<MasaAppConfigureOptions>> _masaAppConfigureOptions;
    private Mock<IEventBus> _eventBus;
    private Mock<IUnitOfWork> _uoW;

    [TestInitialize]
    public void Initialize()
    {
        _options = new();
        _options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        _dispatcherOptions = new();
        _dispatcherOptions
            .Setup(option => option.Value)
            .Returns(() => new IntegrationEventOptions(_options.Object.Services, AppDomain.CurrentDomain.GetAssemblies()));
        _publisher = new();
        _logger = new();
        _eventLog = new();
        _eventLog.Setup(eventLog => eventLog.SaveEventAsync(It.IsAny<IIntegrationEvent>(), null!, default)).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), default)).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsPublishedAsync(It.IsAny<Guid>(), default)).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsFailedAsync(It.IsAny<Guid>(), default)).Verifiable();
        _masaAppConfigureOptions = new();
        _masaAppConfigureOptions.Setup(masaAppConfigureOptions => masaAppConfigureOptions.CurrentValue).Returns(()
            => new MasaAppConfigureOptions()
            {
                AppId = "Test"
            });
        _eventBus = new();
        _uoW = new();
        _uoW.Setup(uoW => uoW.CommitAsync(default)).Verifiable();
        _uoW.Setup(uoW => uoW.Transaction).Returns(() => null!);
        _uoW.Setup(uoW => uoW.UseTransaction).Returns(true);
    }

    [TestMethod]
    public void TestDispatcherOption()
    {
        var services = new ServiceCollection();
        var options = new IntegrationEventOptions(services, new[] { typeof(IntegrationEventBusTest).Assembly });
        Assert.IsTrue(options.Services.Equals(services));
        var allEventTypes = new[] { typeof(IntegrationEventBusTest).Assembly }.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type != typeof(IntegrationEvent) && typeof(IEvent).IsAssignableFrom(type)).ToList();
        Assert.IsTrue(options.AllEventTypes.Count == allEventTypes.Count());
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task TestPublishIntegrationEventAsync(bool useLogger)
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            useLogger ? _logger.Object : null,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "masa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(e => e.SaveEventAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _eventLog.Verify(e => e.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsPublishedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsFailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _publisher.Verify(pub => pub.PublishAsync(@event.Topic, @event, default), Times.Never);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task TestPublishIntegrationEventAndNotUoWAsync(bool useLogger)
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            useLogger ? _logger.Object : null,
            _eventBus.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "masa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(e => e.SaveEventAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventLog.Verify(e => e.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsPublishedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsFailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _publisher.Verify(pub => pub.PublishAsync(@event.Topic, @event, default), Times.Once);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task TestNotUseTransactionAsync(bool useLogger)
    {
        _uoW.Setup(uoW => uoW.UseTransaction).Returns(false);
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            useLogger ? _logger.Object : null,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "masa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(e => e.SaveEventAsync(It.IsAny<IIntegrationEvent>(), It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _eventLog.Verify(e => e.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsPublishedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _eventLog.Verify(e => e.MarkEventAsFailedAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _publisher.Verify(pub => pub.PublishAsync(@event.Topic, @event, default), Times.Once);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task TestSaveEventFailedAsync(bool useLogger)
    {
        _eventLog.Setup(eventLog => eventLog.SaveEventAsync(It.IsAny<IIntegrationEvent>(), null!, default))
            .Callback(() => throw new Exception("custom exception"));
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            useLogger ? _logger.Object : null,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "masa",
            Password = "123456"
        };
        await Assert.ThrowsExceptionAsync<Exception>(async () => await integrationEventBus.PublishAsync(@event), "custom exception");
    }

    [TestMethod]
    public async Task TestPublishLocalEventAsync()
    {
        _eventBus.Setup(eventBus => eventBus.PublishAsync(It.IsAny<CreateUserEvent>(), default)).Verifiable();
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        CreateUserEvent @event = new CreateUserEvent()
        {
            Name = "Tom"
        };
        await integrationEventBus.PublishAsync(@event);

        _eventBus.Verify(eventBus => eventBus.PublishAsync(It.IsAny<CreateUserEvent>(), default), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishEventAndNotEventBusAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            null,
            _uoW.Object);
        CreateUserEvent @event = new CreateUserEvent()
        {
            Name = "Tom"
        };
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            await integrationEventBus.PublishAsync(@event);
        });
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);

        await integrationEventBus.CommitAsync(default);
        _uoW.Verify(uoW => uoW.CommitAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task TestNotUseUowCommitAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            _eventBus.Object,
            null);

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await integrationEventBus.CommitAsync());
    }

    [TestMethod]
    public void TestGetAllEventTypes()
    {
        _dispatcherOptions
            .Setup(option => option.Value)
            .Returns(() => new IntegrationEventOptions(_options.Object.Services, new[] { typeof(IntegrationEventBusTest).Assembly }));
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            null,
            null);

        Assert.IsTrue(integrationEventBus.GetAllEventTypes().Count() == _dispatcherOptions.Object.Value.AllEventTypes.Count());
    }

    [TestMethod]
    public void TestUseEventBusGetAllEventTypes()
    {
        var defaultAssembly = new System.Reflection.Assembly[1] { typeof(IntegrationEventBusTest).Assembly };
        _dispatcherOptions
            .Setup(option => option.Value)
            .Returns(() => new IntegrationEventOptions(_options.Object.Services, defaultAssembly));
        var allEventType = defaultAssembly
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
        _eventBus.Setup(eventBus => eventBus.GetAllEventTypes()).Returns(() => allEventType).Verifiable();
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _masaAppConfigureOptions.Object,
            _logger.Object,
            _eventBus.Object,
            null);

        Assert.IsTrue(integrationEventBus.GetAllEventTypes().Count() == _dispatcherOptions.Object.Value.AllEventTypes.Count());
        Assert.IsTrue(integrationEventBus.GetAllEventTypes().Count() == allEventType.Count());
    }

    [TestMethod]
    public void TestAddIntegrationEventBusReturnThrowNoImplementing()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<NotSupportedException>(() => services.AddIntegrationEventBus<CustomIntegrationEventLogService>());
    }

    [TestMethod]
    public void TestAddMultiIntegrationEventBusReturnIntegrationEventBusCountEqual1()
    {
        var services = new ServiceCollection();
        services.AddIntegrationEventBus<CustomIntegrationEventLogService>(dispatcherOptions =>
        {
            Mock<IPublisher> publisher = new();
            dispatcherOptions.Services.TryAddSingleton(publisher.Object);
        }).AddIntegrationEventBus<CustomIntegrationEventLogService>(dispatcherOptions =>
        {
            Mock<IPublisher> publisher = new();
            dispatcherOptions.Services.TryAddSingleton(publisher.Object);
        });
        var serviceProvider = services.BuildServiceProvider();
        var integrationEventBuses = serviceProvider.GetServices<IIntegrationEventBus>();
        Assert.IsTrue(integrationEventBuses.Count() == 1);
    }
}
