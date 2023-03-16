// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class ProcessorTest
{
    private IServiceProvider _serviceProvider;
    private IOptions<IntegrationEventOptions> _options;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        MockPublisher(services);
        services.AddIntegrationEventBus<CustomIntegrationEventLogService>();
        _serviceProvider = services.BuildServiceProvider();
        _options = _serviceProvider.GetRequiredService<IOptions<IntegrationEventOptions>>();
    }

    [TestMethod]
    public void DeleteLocalQueueExpiresProcessorDelayTestAsync()
    {
        ProcessorBase processor = new DeleteLocalQueueExpiresProcessor(_options);
        Assert.IsTrue(processor.Delay == _options.Value.CleaningLocalQueueExpireInterval);
    }

    [TestMethod]
    public void DeletePublishedExpireEventDelayTestAsync()
    {
        ProcessorBase processor = new DeletePublishedExpireEventProcessor(_serviceProvider, _options);
        Assert.IsTrue(processor.Delay == _options.Value.CleaningExpireInterval);
    }

    [TestMethod]
    public void RetryByDataProcessorDelayTestAsync()
    {
        ProcessorBase processor = new RetryByDataProcessor(_serviceProvider, _options);
        Assert.IsTrue(processor.Delay == _options.Value.FailedRetryInterval);
    }

    [TestMethod]
    public async Task RetryByDataProcessorExecuteTestAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        Mock<IPublisher> publisher = new();
        publisher.Setup(client
            => client.PublishAsync(It.IsAny<string>(), It.IsAny<IIntegrationEvent>(), cancellationTokenSource.Token));
        services.AddScoped(_ => publisher.Object);

        Mock<IIntegrationEventLogService> integrationEventLogService = new();
        RegisterUserIntegrationEvent @event = new RegisterUserIntegrationEvent();

        integrationEventLogService.Setup(service
            => service.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), cancellationTokenSource.Token)).Verifiable();
        integrationEventLogService.Setup(service => service.MarkEventAsPublishedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token))
            .Verifiable();

        List<IntegrationEventLog> list = new List<IntegrationEventLog>()
        {
            new(@event, Guid.Empty),
            new(@event, Guid.Empty)
        };
        list.ForEach(item =>
        {
            item.DeserializeJsonContent(typeof(RegisterUserIntegrationEvent));
        });
        integrationEventLogService.Setup(service =>
                service.RetrieveEventLogsFailedToPublishAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                    cancellationTokenSource.Token))
            .ReturnsAsync(() => list)
            .Verifiable();
        services.AddScoped(_ => integrationEventLogService.Object);

        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(u => u.CommitAsync(cancellationTokenSource.Token)).Verifiable();
        uoW.Setup(u => u.ServiceProvider).Returns(services.BuildServiceProvider()).Verifiable();
        services.AddScoped(_ => uoW.Object);

        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        unitOfWorkManager.Setup(uoWManager => uoWManager.CreateDbContext(It.IsAny<MasaDbContextConfigurationOptions>())).Returns(uoW.Object)
            .Verifiable();
        services.AddSingleton(_ => unitOfWorkManager.Object);
        services.AddSingleton<ILocalMessageDbConnectionStringProvider, LocalMessageDbConnectionStringProvider>();

        Mock<IOptions<IntegrationEventOptions>> options = new();
        options.Setup(opt => opt.Value).Returns(new IntegrationEventOptions(services, AppDomain.CurrentDomain.GetAssemblies()));
        MasaAppConfigureOptions masaAppConfigureOptions = new()
        {
            AppId = "test"
        };

        var serviceProvider = services.BuildServiceProvider();
        RetryByDataProcessor retryByDataProcessor = new(
            serviceProvider,
            options.Object,
            Mock.Of<IOptionsMonitor<MasaAppConfigureOptions>>(a => a.CurrentValue == masaAppConfigureOptions),
            serviceProvider.GetService<ILogger<RetryByDataProcessor>>());
        await retryByDataProcessor.ExecuteAsync(cancellationTokenSource.Token);

        integrationEventLogService.Verify(
            service => service.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), cancellationTokenSource.Token),
            Times.Exactly(2));
        integrationEventLogService.Verify(service => service.MarkEventAsPublishedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token),
            Times.Exactly(2));
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task RetryByDataProcessorExecute2TestAsync(bool useLogger)
    {
        var services = new ServiceCollection();
        if (useLogger) services.AddLogging();

        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(1000);

        Mock<IIntegrationEventLogService> integrationEventLogService = new();
        integrationEventLogService.Setup(service
                => service.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), cancellationTokenSource.Token))
            .Verifiable();
        integrationEventLogService.Setup(service => service.MarkEventAsPublishedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token))
            .Verifiable();
        integrationEventLogService.Setup(service => service.MarkEventAsFailedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token))
            .Verifiable();

        List<IntegrationEventLog> list = new List<IntegrationEventLog>()
        {
            new(new RegisterUserIntegrationEvent(), Guid.Empty),
            new(new PaySuccessedIntegrationEvent(Guid.NewGuid().ToString()), Guid.Empty)
        };
        for (int index = 0; index < list.Count; index++)
        {
            if (index == 0)
                list[index].DeserializeJsonContent(typeof(RegisterUserIntegrationEvent));
            else
                list[index].DeserializeJsonContent(typeof(PaySuccessedIntegrationEvent));
        }

        Mock<IPublisher> publisher = new();
        publisher.Setup(client
                => client.PublishAsync(nameof(RegisterUserIntegrationEvent), It.IsAny<IIntegrationEvent>(),
                    cancellationTokenSource.Token))
            .Throws(new Exception("custom exception"));
        publisher.Setup(client
                => client.PublishAsync(nameof(PaySuccessedIntegrationEvent), It.IsAny<IIntegrationEvent>(),
                    cancellationTokenSource.Token))
            .Throws(new UserFriendlyException("custom exception"));
        services.AddScoped(_ => publisher.Object);

        integrationEventLogService.Setup(service =>
                service.RetrieveEventLogsFailedToPublishAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                    cancellationTokenSource.Token))
            .ReturnsAsync(() => list)
            .Verifiable();
        services.AddScoped(_ => integrationEventLogService.Object);

        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(u => u.CommitAsync(cancellationTokenSource.Token)).Verifiable();
        uoW.Setup(u => u.ServiceProvider).Returns(services.BuildServiceProvider()).Verifiable();
        services.AddScoped(_ => uoW.Object);

        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        unitOfWorkManager.Setup(uoWManager => uoWManager.CreateDbContext(It.IsAny<MasaDbContextConfigurationOptions>())).Returns(uoW.Object)
            .Verifiable();
        services.AddSingleton(_ => unitOfWorkManager.Object);

        services.AddSingleton<ILocalMessageDbConnectionStringProvider, LocalMessageDbConnectionStringProvider>();

        Mock<IOptions<IntegrationEventOptions>> options = new();
        options.Setup(opt => opt.Value).Returns(new IntegrationEventOptions(services, AppDomain.CurrentDomain.GetAssemblies()));
        MasaAppConfigureOptions masaAppConfigureOptions = new()
        {
            AppId = "test"
        };

        var serviceProvider = services.BuildServiceProvider();
        RetryByDataProcessor retryByDataProcessor = new(
            serviceProvider,
            options.Object,
            Mock.Of<IOptionsMonitor<MasaAppConfigureOptions>>(a => a.CurrentValue == masaAppConfigureOptions),
            serviceProvider.GetService<ILogger<RetryByDataProcessor>>());
        await retryByDataProcessor.ExecuteAsync(cancellationTokenSource.Token);

        integrationEventLogService.Verify(
            service => service.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), cancellationTokenSource.Token),
            Times.Exactly(2));
        integrationEventLogService.Verify(service => service.MarkEventAsPublishedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token),
            Times.Never);
        integrationEventLogService.Verify(service => service.MarkEventAsFailedAsync(It.IsAny<Guid>(), cancellationTokenSource.Token),
            Times.Exactly(1));
    }

    [TestMethod]
    public void RetryByLocalQueueProcessorDelayTestAsync()
    {
        ProcessorBase processor = new RetryByLocalQueueProcessor(_serviceProvider, _options);
        Assert.IsTrue(processor.Delay == _options.Value.LocalFailedRetryInterval);
    }

    [TestMethod]
    public async Task DeletePublishedExpireEventProcessorExecuteTestAsync()
    {
        Mock<IIntegrationEventLogService> integrationEventLogService = new();
        integrationEventLogService.Setup(service => service.DeleteExpiresAsync(It.IsAny<DateTime>(), It.IsAny<int>(), default))
            .Verifiable();
        _options.Value.Services.AddScoped(_ => integrationEventLogService.Object);

        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(uow => uow.ServiceProvider).Returns(_options.Value.Services.BuildServiceProvider()).Verifiable();

        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        unitOfWorkManager.Setup(uoWManager => uoWManager.CreateDbContext(It.IsAny<MasaDbContextConfigurationOptions>())).Returns(uoW.Object)
            .Verifiable();
        _options.Value.Services.AddSingleton(_ => unitOfWorkManager.Object);

        _options.Value.Services.AddSingleton<ILocalMessageDbConnectionStringProvider, LocalMessageDbConnectionStringProvider>();

        var processor = new DeletePublishedExpireEventProcessor(_options.Value.Services.BuildServiceProvider(), _options);
        await processor.ExecuteAsync(default);

        integrationEventLogService.Verify(service => service.DeleteExpiresAsync(It.IsAny<DateTime>(), It.IsAny<int>(), default),
            Times.Once);
    }

    [TestMethod]
    public async Task InfiniteLoopProcessorExecuteTestAsync()
    {
        Mock<IProcessor> processor = new();
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(3000);
        processor.Setup(pro => pro.ExecuteAsync(cancellationTokenSource.Token)).Verifiable();

        InfiniteLoopProcessor infiniteLoopProcessor = new InfiniteLoopProcessor(_serviceProvider, processor.Object);
        await infiniteLoopProcessor.ExecuteAsync(cancellationTokenSource.Token);

        processor.Verify(pro => pro.ExecuteAsync(cancellationTokenSource.Token), Times.AtLeastOnce);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task DefaultHostedServiceTestAsync(bool useLogger)
    {
        var services = new ServiceCollection();
        MockPublisher(services);
        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        services.AddSingleton(_ => unitOfWorkManager.Object);
        if (useLogger) services.AddLogging();
        services.AddScoped<IProcessor, CustomProcessor>();
        services.AddIntegrationEventBus<CustomIntegrationEventLogService>(opt =>
        {
            opt.CleaningLocalQueueExpireInterval = 1;
            opt.CleaningExpireInterval = 1;
            opt.FailedRetryInterval = 1;
            opt.LocalFailedRetryInterval = 1;
        });
        var serviceProvider = services.BuildServiceProvider();
        var hostedService = serviceProvider.GetService<IProcessingServer>();
        Assert.IsNotNull(hostedService);
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(5000);
        await hostedService.ExecuteAsync(cancellationTokenSource.Token);

        Assert.IsTrue(CustomProcessor.Times > 0);
    }

    private void MockPublisher(IServiceCollection services)
    {
        Mock<IPublisher> publisher = new();
        publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<IIntegrationEvent>(), default)).Verifiable();
        services.AddSingleton(publisher.Object);
    }
}
