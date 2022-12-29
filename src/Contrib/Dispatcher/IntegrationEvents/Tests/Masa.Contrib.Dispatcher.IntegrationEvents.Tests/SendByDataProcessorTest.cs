// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class SendByDataProcessorTest
{
    private readonly IServiceCollection _services;
    private readonly CustomSendByDataProcessor _processor;

    public SendByDataProcessorTest()
    {
        _services = new ServiceCollection();
        var dispatcherOptions =
            Microsoft.Extensions.Options.Options.Create(new IntegrationEventOptions(_services, new[] { this.GetType().Assembly }));
        _processor = new CustomSendByDataProcessor(_services.BuildServiceProvider(), dispatcherOptions);
    }

    [TestMethod]
    public async Task TestPublishEventByPublishIsFailedAsync()
    {
        Mock<IIntegrationEventLogService> logService = new();
        Mock<IDbConnectionStringProvider> dbConnectionStringProvider = new();
        dbConnectionStringProvider
            .Setup(p => p.DbContextOptionsList)
            .Returns(() => new List<MasaDbContextConfigurationOptions>()
            {
                new("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity")
            });

        var eventLogs = new List<IntegrationEventLog>()
        {
            new(new RegisterUserEvent(), Guid.NewGuid())
        };
        foreach (var log in eventLogs)
        {
            log.DeserializeJsonContent(typeof(RegisterUserEvent));
        }
        logService
            .Setup(l => l.RetrieveEventLogsPendingToPublishAsync(20, default))
            .ReturnsAsync(() => eventLogs);
        logService
            .Setup(l => l.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), default))
            .Verifiable();
        logService
            .Setup(l => l.MarkEventAsFailedAsync(It.IsAny<Guid>(), default))
            .Verifiable();

        Mock<IPublisher> publisher = new();
        publisher
            .Setup(p => p.PublishAsync(It.IsAny<string>(), eventLogs[0].Event, default))
            .Returns(() => throw new NotSupportedException());
        _services.AddSingleton(_ => publisher.Object);
        _services.AddScoped(_ => logService.Object);

        await _processor.TestExecuteAsync(_services.BuildServiceProvider(), CancellationToken.None);

        logService.Verify(l => l.RetrieveEventLogsPendingToPublishAsync(It.IsAny<int>(), default), Times.Once);
        logService.Verify(l => l.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), default), Times.Once);
        logService.Verify(l => l.MarkEventAsPublishedAsync(It.IsAny<Guid>(), default), Times.Never);
        logService.Verify(l => l.MarkEventAsFailedAsync(It.IsAny<Guid>(), default), Times.Once);
        publisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<IIntegrationEvent>(), default), Times.Once);
    }

    [TestMethod]
    public async Task TestPublishEventByPublishIsSuccessedAsync()
    {
        Mock<IIntegrationEventLogService> logService = new();
        Mock<IDbConnectionStringProvider> dbConnectionStringProvider = new();
        dbConnectionStringProvider
            .Setup(p => p.DbContextOptionsList)
            .Returns(() => new List<MasaDbContextConfigurationOptions>()
            {
                new("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity")
            });

        var eventLogs = new List<IntegrationEventLog>()
        {
            new(new RegisterUserEvent(), Guid.NewGuid())
        };
        foreach (var log in eventLogs)
        {
            log.DeserializeJsonContent(typeof(RegisterUserEvent));
        }
        logService
            .Setup(l => l.RetrieveEventLogsPendingToPublishAsync(20, default))
            .ReturnsAsync(() => eventLogs);
        logService
            .Setup(l => l.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), default))
            .Verifiable();
        logService
            .Setup(l => l.MarkEventAsFailedAsync(It.IsAny<Guid>(), default))
            .Verifiable();

        Mock<IPublisher> publisher = new();
        publisher
            .Setup(p => p.PublishAsync(It.IsAny<string>(), eventLogs[0].Event, default))
            .Verifiable();
        _services.AddSingleton(_ => publisher.Object);
        _services.AddScoped(_ => logService.Object);

        await _processor.TestExecuteAsync(_services.BuildServiceProvider(), CancellationToken.None);

        logService.Verify(l => l.RetrieveEventLogsPendingToPublishAsync(It.IsAny<int>(), default), Times.Once);
        logService.Verify(l => l.MarkEventAsInProgressAsync(It.IsAny<Guid>(), It.IsAny<int>(), default), Times.Once);
        logService.Verify(l => l.MarkEventAsPublishedAsync(It.IsAny<Guid>(), default), Times.Once);
        logService.Verify(l => l.MarkEventAsFailedAsync(It.IsAny<Guid>(), default), Times.Never);
        publisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<IIntegrationEvent>(), default), Times.Once);
    }
}
