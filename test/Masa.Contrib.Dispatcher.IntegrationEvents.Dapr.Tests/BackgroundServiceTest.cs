namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

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

        var integrationEventHostedService = new CustomizeIntegrationEventHostedService(processingServer.Object);
        await integrationEventHostedService.TestExecuteAsync(default);

        processingServer.Verify(service => service.ExecuteAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task IntegrationEventHostedServiceAndUseLoggerTask()
    {
        Mock<IProcessingServer> processingServer = new();
        processingServer.Setup(service => service.ExecuteAsync(default)).Verifiable();

        var integrationEventHostedService = new CustomizeIntegrationEventHostedService(processingServer.Object, new NullLoggerFactory());
        await integrationEventHostedService.TestExecuteAsync(default);

        processingServer.Verify(service => service.ExecuteAsync(default), Times.Once);
    }
}
