// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class ProcessorTest
{
    private IServiceProvider _serviceProvider;
    private IOptions<DispatcherOptions> _options;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddDaprEventBus<CustomizeIntegrationEventLogService>();
        _serviceProvider = services.BuildServiceProvider();
        _options = _serviceProvider.GetRequiredService<IOptions<DispatcherOptions>>();
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
    public void RetryByLocalQueueProcessorDelayTestAsync()
    {
        ProcessorBase processor = new RetryByLocalQueueProcessor(_serviceProvider, _options);
        Assert.IsTrue(processor.Delay == _options.Value.LocalFailedRetryInterval);
    }

    [TestMethod]
    public async Task DeletePublishedExpireEventProcessorExecuteTestAsync()
    {
        Mock<IIntegrationEventLogService> integrationEventLogService = new();
        integrationEventLogService.Setup(service => service.DeleteExpiresAsync(It.IsAny<DateTime>(), It.IsAny<int>(), default)).Verifiable();
        _options.Value.Services.AddScoped(_ => integrationEventLogService.Object);

        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(uow => uow.ServiceProvider).Returns(_options.Value.Services.BuildServiceProvider()).Verifiable();

        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        unitOfWorkManager.Setup(uoWManager => uoWManager.CreateDbContext(It.IsAny<MasaDbContextConfigurationOptions>())).Returns(uoW.Object).Verifiable();
        _options.Value.Services.AddSingleton(_ => unitOfWorkManager.Object);

        Mock<IDbConnectionStringProvider> dataConnectionStringProvider = new();
        dataConnectionStringProvider.Setup(provider => provider.DbContextOptionsList).Returns(new List<MasaDbContextConfigurationOptions>()
        {
            new(string.Empty)
        }).Verifiable();
        _options.Value.Services.AddSingleton(_ => dataConnectionStringProvider.Object);

        var processor = new DeletePublishedExpireEventProcessor(_options.Value.Services.BuildServiceProvider(), _options);
        await processor.ExecuteAsync(default);

        integrationEventLogService.Verify(service => service.DeleteExpiresAsync(It.IsAny<DateTime>(), It.IsAny<int>(), default), Times.Once);
    }

    [TestMethod]
    public void SetGetCurrentTime()
    {
        var services = new ServiceCollection();
        DateTime dateNow = DateTime.Now;
        services.AddDaprEventBus<CustomizeIntegrationEventLogService>(opt =>
        {
            opt.GetCurrentTime = () => dateNow;
        });

        var dispatcherOption = services.BuildServiceProvider().GetRequiredService<IOptions<DispatcherOptions>>();
        Assert.IsNotNull(dispatcherOption);
        Assert.IsTrue((dispatcherOption!.Value.GetCurrentTime!.Invoke() - DateTime.Now).TotalMinutes < 1);
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

    [TestMethod]
    public async Task InfiniteLoopProcessorExecuteAndUseLoggerTestAsync()
    {
        Mock<IProcessor> processor = new();
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(3000);
        processor.Setup(pro => pro.ExecuteAsync(cancellationTokenSource.Token)).Verifiable();

        InfiniteLoopProcessor infiniteLoopProcessor =
            new InfiniteLoopProcessor(_serviceProvider, processor.Object);
        await infiniteLoopProcessor.ExecuteAsync(cancellationTokenSource.Token);

        processor.Verify(pro => pro.ExecuteAsync(cancellationTokenSource.Token), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task DefaultHostedServiceTestAsync()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        services.AddSingleton(_ => unitOfWorkManager.Object);
        services.AddScoped<IProcessor, CustomizeProcessor>();
        services.AddDaprEventBus<CustomizeIntegrationEventLogService>(opt =>
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

        Assert.IsTrue(CustomizeProcessor.Times > 0);
    }

    [TestMethod]
    public async Task DefaultHostedServiceAndUseLoggerTestAsync()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        services.AddSingleton(_ => unitOfWorkManager.Object);
        services.AddLogging();
        services.AddScoped<IProcessor, CustomizeProcessor>();
        services.AddDaprEventBus<CustomizeIntegrationEventLogService>(opt =>
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

        Assert.IsTrue(CustomizeProcessor.Times > 0);
    }
}
