// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class BackgroundServiceTest
{
    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public async Task IntegrationEventHostedServiceTask()
    {
        Mock<IProcessingServer> processingServer = new();
        processingServer.Setup(service => service.ExecuteAsync(default)).Verifiable();

        var integrationEventHostedService = new CustomIntegrationEventHostedService(processingServer.Object);
        await integrationEventHostedService.TestExecuteAsync(default);

        processingServer.Verify(service => service.ExecuteAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task IntegrationEventHostedServiceAndUseLoggerTask()
    {
        Mock<IProcessingServer> processingServer = new();
        processingServer.Setup(service => service.ExecuteAsync(default)).Verifiable();

        var integrationEventHostedService = new CustomIntegrationEventHostedService(processingServer.Object, new NullLoggerFactory());
        await integrationEventHostedService.TestExecuteAsync(default);

        processingServer.Verify(service => service.ExecuteAsync(default), Times.Once);
    }
}
