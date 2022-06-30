// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class DispatcherOptionTest
{
    private DispatcherOptions _options;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        _options = new DispatcherOptions(services, AppDomain.CurrentDomain.GetAssemblies());
    }

    [TestMethod]
    public void TestEmptyPubSub()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _options.PubSubName = "");
    }

    [TestMethod]
    public void TestSetPubSub()
    {
        Assert.IsTrue(_options.PubSubName == "pubsub");
        _options.PubSubName = "pubsub2";
        Assert.IsTrue(_options.PubSubName == "pubsub2");
    }

    [TestMethod]
    public void UseDaprEventBus()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        Mock<IDistributedDispatcherOptions> distributedDispatcherOptions = new();
        distributedDispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        distributedDispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        distributedDispatcherOptions.Object.UseDaprEventBus<CustomizeIntegrationEventLogService>("pubsub2");
        var serviceProvider = services.BuildServiceProvider();

        var integrationEventBus = serviceProvider.GetService<IIntegrationEventBus>();
        Assert.IsNotNull(integrationEventBus);

        var publisher = serviceProvider.GetService<IPublisher>();
        Assert.IsNotNull(publisher);

        var field = publisher.GetType().GetField("_pubSubName", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field);
        var fieldValue = field.GetValue(publisher);
        Assert.IsNotNull(fieldValue);
        Assert.IsTrue(fieldValue.ToString() == "pubsub2");
    }
}
