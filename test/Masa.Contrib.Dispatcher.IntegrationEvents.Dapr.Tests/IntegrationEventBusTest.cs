// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class IntegrationEventBusTest
{
    private Mock<IDispatcherOptions> _options;
    private Mock<IOptions<DispatcherOptions>> _dispatcherOptions;
    private Mock<IIntegrationEventLogService> _eventLog;
    private Mock<IOptionsMonitor<AppConfig>> _appConfig;
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
    public void TestAddMultDaprEventBus()
    {
        var services = new ServiceCollection();
        Mock<IDistributedDispatcherOptions> distributedDispatcherOptions = new();
        distributedDispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        distributedDispatcherOptions.Setup(option => option.Assemblies).Returns(AppDomain.CurrentDomain.GetAssemblies()).Verifiable();
        distributedDispatcherOptions.Object
            .UseDaprEventBus<CustomizeIntegrationEventLogService>()
            .UseDaprEventBus<CustomizeIntegrationEventLogService>();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IIntegrationEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestAddDaprEventBus()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddDaprEventBus<CustomizeIntegrationEventLogService>();
        var serviceProvider = services.BuildServiceProvider();
        var integrationEventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        Assert.IsNotNull(integrationEventBus);
    }

    [TestMethod]
    public void TestNotUseLoggerAndUoW()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        services
            .AddDaprEventBus<
                CustomizeIntegrationEventLogService>(); //The logger cannot be mocked and cannot verify that the logger is executed only once

        var serviceProvider = services.BuildServiceProvider();
        var integrationEventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        Assert.IsNotNull(integrationEventBus);
    }

    [TestMethod]
    public void TestUseLogger()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddDaprEventBus<CustomizeIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), option =>
        {
            option.PubSubName = "pubsub";
        });
        var serviceProvider = services.BuildServiceProvider();
        var integrationEventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        Assert.IsNotNull(integrationEventBus);
    }

    [TestMethod]
    public void TestAddDaprEventBusAndNullServicesAsync()
    {
        IServiceCollection services = null!;
        Mock<IDistributedDispatcherOptions> distributedDispatcherOptions = new();
        distributedDispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        distributedDispatcherOptions.Setup(option => option.Assemblies).Returns(AppDomain.CurrentDomain.GetAssemblies()).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() =>
                distributedDispatcherOptions.Object.UseDaprEventBus<CustomizeIntegrationEventLogService>(),
            $"Value cannot be null. (Parameter '{nameof(_options.Object.Services)}')");
    }

    [TestMethod]
    public void TestUseDaprReturnNotNull()
    {
        var services = new ServiceCollection();
        services.AddIntegrationEventBus<CustomizeIntegrationEventLogService>(opt =>
        {
            opt.UseDapr();
        });
        Mock<IIntegrationEventLogService> eventLogService = new();
        services.AddScoped(_ => eventLogService.Object);

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetService<IPublisher>();
        Assert.IsNotNull(publisher);
        var integrationEventBus = serviceProvider.GetService<IIntegrationEventBus>();
        Assert.IsNotNull(integrationEventBus);
    }
}
