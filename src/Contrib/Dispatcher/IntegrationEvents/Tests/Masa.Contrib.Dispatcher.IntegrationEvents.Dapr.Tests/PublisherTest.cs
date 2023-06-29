// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class PublisherTest
{
    private string _pubSubName;

    [TestInitialize]
    public void Initialize()
    {
        _pubSubName = "pubsub";
    }

    [DataTestMethod]
    [DataRow("daprAppId", true, false, 0)]
    [DataRow("daprAppId", false, false, 0)]
    [DataRow("", true, true, 1)]
    [DataRow("", false, false, 0)]
    [DataRow(null, true, true, 1)]
    [DataRow(null, false, false, 0)]
    public async Task TestPublishAsync(string daprAppId, bool enableIsolation, bool isThrowException, int writeLogTimer)
    {
        Mock<ILogger<Publisher>> logger = new();
        logger.Setup(log => log.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!)).Verifiable();
        Mock<DaprClient> client = new();
        string topicName = nameof(RegisterUserIntegrationEvent);
        client.Setup(c => c.PublishEventAsync(_pubSubName, topicName, It.IsAny<object>(), It.IsAny<CancellationToken>()));
        var services = new ServiceCollection();
        if (enableIsolation)
        {
            services.Configure<IsolationOptions>(options => options.MultiEnvironmentName = "env");
        }

        services.AddSingleton(client.Object);
        services.AddSingleton(logger.Object);
        var serviceProvider = services.BuildServiceProvider();

        Publisher publisher;

        if (isThrowException)
        {
            Assert.ThrowsException<MasaArgumentException>(()
                => publisher = new Publisher(serviceProvider, _pubSubName, "appId", daprAppId));
            logger.Verify(c => c.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), ((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!), Times.Exactly(writeLogTimer));
            return;
        }

        publisher= new Publisher(serviceProvider, _pubSubName, "appId", daprAppId);
        var registerEvent = new RegisterUserIntegrationEvent();
        await publisher.PublishAsync(topicName, registerEvent, null, CancellationToken.None);
        client.Verify(
            c => c.PublishEventAsync<object>(_pubSubName, topicName, It.IsAny<RegisterUserIntegrationEvent>(), CancellationToken.None),
            Times.Once);
        client.Verify(
            c => c.PublishEventAsync(_pubSubName, topicName, It.IsAny<MasaCloudEvent<IntegrationEventMessage>>(), CancellationToken.None),
            Times.Never);
    }

    [TestMethod]
    public async Task TestPublishAsyncByEventExpand()
    {
        Mock<DaprClient> client = new();
        string topicName = nameof(RegisterUserIntegrationEvent);
        client.Setup(c => c.PublishEventAsync(_pubSubName, topicName, It.IsAny<MasaCloudEvent<IntegrationEventMessage>>(),
            It.IsAny<CancellationToken>()));
        var services = new ServiceCollection();
        services.AddSingleton(client.Object);
        var serviceProvider = services.BuildServiceProvider();
        var publisher = new Publisher(serviceProvider, _pubSubName, "appId", "daprAppId");
        var registerEvent = new RegisterUserIntegrationEvent();

        await publisher.PublishAsync(topicName, registerEvent, new IntegrationEventExpand()
        {
            Isolation = new Dictionary<string, string>()
            {
                {
                    "env", "test"
                }
            }
        }, CancellationToken.None);
        client.Verify(
            c => c.PublishEventAsync<object>(_pubSubName, topicName, It.IsAny<RegisterUserIntegrationEvent>(), CancellationToken.None),
            Times.Never);
        client.Verify(
            c => c.PublishEventAsync(_pubSubName, topicName, It.IsAny<MasaCloudEvent<IntegrationEventMessage>>(), CancellationToken.None),
            Times.Once);
    }
}
