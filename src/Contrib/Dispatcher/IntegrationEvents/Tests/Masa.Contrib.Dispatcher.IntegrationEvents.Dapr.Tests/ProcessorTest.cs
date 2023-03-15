// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class ProcessorTest
{
    private IServiceProvider _serviceProvider;
    private IOptions<DaprIntegrationEventOptions> _options;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddDaprEventBus<CustomIntegrationEventLogService>();
        _serviceProvider = services.BuildServiceProvider();
        _options = Microsoft.Extensions.Options.Options.Create(new DaprIntegrationEventOptions(services, AppDomain.CurrentDomain.GetAssemblies()));
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

        Mock<ILocalMessageDbConnectionStringProvider> dataConnectionStringProvider = new();
        dataConnectionStringProvider.Setup(provider => provider.DbContextOptionsList).Returns(new List<MasaDbContextConfigurationOptions>()
        {
            new(string.Empty)
        }).Verifiable();
        _options.Value.Services.AddSingleton(_ => dataConnectionStringProvider.Object);

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
        services.AddScoped<IProcessor, CustomProcessor>();
        services.AddDaprEventBus<CustomIntegrationEventLogService>(opt =>
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

    [TestMethod]
    public async Task DefaultHostedServiceAndUseLoggerTestAsync()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWorkManager> unitOfWorkManager = new();
        services.AddSingleton(_ => unitOfWorkManager.Object);
        services.AddLogging();
        services.AddScoped<IProcessor, CustomProcessor>();
        services.AddDaprEventBus<CustomIntegrationEventLogService>(opt =>
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
}
