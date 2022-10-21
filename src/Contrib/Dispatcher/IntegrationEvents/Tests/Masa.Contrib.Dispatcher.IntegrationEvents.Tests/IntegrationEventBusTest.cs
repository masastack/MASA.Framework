// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class IntegrationEventBusTest
{
    private Mock<IDispatcherOptions> _options;
    private Mock<IOptions<DispatcherOptions>> _dispatcherOptions;
    private Mock<IPublisher> _publisher;
    private Mock<ILogger<IntegrationEventBus>> _logger;
    private Mock<IIntegrationEventLogService> _eventLog;
    private Mock<IOptionsMonitor<AppConfig>> _appConfig;
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
            .Returns(() => new DispatcherOptions(_options.Object.Services, AppDomain.CurrentDomain.GetAssemblies()));
        _publisher = new();
        _logger = new();
        _eventLog = new();
        _eventLog.Setup(eventLog => eventLog.SaveEventAsync(It.IsAny<IIntegrationEvent>(), null!)).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsInProgressAsync(It.IsAny<Guid>())).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsPublishedAsync(It.IsAny<Guid>())).Verifiable();
        _eventLog.Setup(eventLog => eventLog.MarkEventAsFailedAsync(It.IsAny<Guid>())).Verifiable();
        _appConfig = new();
        _appConfig.Setup(appConfig => appConfig.CurrentValue).Returns(() => new AppConfig()
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
        DispatcherOptions options;

        Assert.ThrowsException<ArgumentException>(() =>
        {
            options = new DispatcherOptions(services, null!);
        });
        Assert.ThrowsException<ArgumentException>(() =>
        {
            options = new DispatcherOptions(services, Array.Empty<Assembly>());
        });
        options = new DispatcherOptions(services, new[] { typeof(IntegrationEventBusTest).Assembly });
        Assert.IsTrue(options.Services.Equals(services));
        var allEventTypes = new[] { typeof(IntegrationEventBusTest).Assembly }.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type != typeof(IntegrationEvent) && typeof(IEvent).IsAssignableFrom(type)).ToList();
        Assert.IsTrue(options.AllEventTypes.Count == allEventTypes.Count());
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _publisher.Verify(pub => pub.PublishAsync(@event.Topic, @event, default),
            Times.Once);
    }

    [TestMethod]
    public async Task TestNotUseUoWAndLoggerAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            null,
            _eventBus.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(eventLog => eventLog.MarkEventAsInProgressAsync(@event.GetEventId()), Times.Never);
        _publisher.Verify(client => client.PublishAsync(@event.Topic, @event, default),
            Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsPublishedAsync(@event.GetEventId()), Times.Never);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsFailedAsync(@event.GetEventId()), Times.Never);
    }

    [TestMethod]
    public async Task TestNotUseTransactionAsync()
    {
        _uoW.Setup(uoW => uoW.UseTransaction).Returns(false);
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(eventLog => eventLog.MarkEventAsInProgressAsync(@event.GetEventId()), Times.Never);
        _publisher.Verify(client => client.PublishAsync(@event.Topic, @event, default),
            Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsPublishedAsync(@event.GetEventId()), Times.Never);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsFailedAsync(@event.GetEventId()), Times.Never);
    }

    [TestMethod]
    public async Task TestUseTranscationAndNotUseLoggerAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            null,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(eventLog => eventLog.MarkEventAsInProgressAsync(@event.GetEventId()), Times.Once);
        _publisher.Verify(client => client.PublishAsync(@event.Topic, @event, default),
            Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsPublishedAsync(@event.GetEventId()), Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsFailedAsync(@event.GetEventId()), Times.Never);
    }

    [TestMethod]
    public async Task TestSaveEventFailedAndNotUseLoggerAsync()
    {
        _eventLog.Setup(eventLog => eventLog.SaveEventAsync(It.IsAny<IIntegrationEvent>(), null!))
            .Callback(() => throw new Exception("custom exception"));
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            null,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await Assert.ThrowsExceptionAsync<Exception>(async () => await integrationEventBus.PublishAsync(@event), "custom exception");
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventAndFailedAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456"
        };
        _eventLog.Setup(eventLog => eventLog.MarkEventAsPublishedAsync(It.IsAny<Guid>())).Throws<Exception>();
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _eventLog.Verify(eventLog => eventLog.MarkEventAsInProgressAsync(@event.GetEventId()), Times.Once);
        _publisher.Verify(client => client.PublishAsync(@event.Topic, @event, default),
            Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsPublishedAsync(@event.GetEventId()), Times.Once);
        _eventLog.Verify(eventLog => eventLog.MarkEventAsFailedAsync(@event.GetEventId()), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventAndNotUoWAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent()
        {
            Account = "lisa",
            Password = "123456",
            UnitOfWork = _uoW.Object
        };
        _publisher.Setup(client => client.PublishAsync(@event.Topic, @event, default))
            .Verifiable();
        await integrationEventBus.PublishAsync(@event);

        _publisher.Verify(pub => pub.PublishAsync(@event.Topic, @event, default),
            Times.Once);
    }

    [TestMethod]
    public async Task TestPublishEventAsync()
    {
        _eventBus.Setup(eventBus => eventBus.PublishAsync(It.IsAny<CreateUserEvent>())).Verifiable();
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
            _logger.Object,
            _eventBus.Object,
            _uoW.Object);
        CreateUserEvent @event = new CreateUserEvent()
        {
            Name = "Tom"
        };
        await integrationEventBus.PublishAsync(@event);

        _eventBus.Verify(eventBus => eventBus.PublishAsync(It.IsAny<CreateUserEvent>()), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishEventAndNotEventBusAsync()
    {
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
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
            _appConfig.Object,
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
            _appConfig.Object,
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
            .Returns(() => new DispatcherOptions(_options.Object.Services, new[] { typeof(IntegrationEventBusTest).Assembly }));
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
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
            .Returns(() => new DispatcherOptions(_options.Object.Services, defaultAssembly));
        var allEventType = defaultAssembly
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
        _eventBus.Setup(eventBus => eventBus.GetAllEventTypes()).Returns(() => allEventType).Verifiable();
        var integrationEventBus = new IntegrationEventBus(
            _dispatcherOptions.Object,
            _publisher.Object,
            _eventLog.Object,
            _appConfig.Object,
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
