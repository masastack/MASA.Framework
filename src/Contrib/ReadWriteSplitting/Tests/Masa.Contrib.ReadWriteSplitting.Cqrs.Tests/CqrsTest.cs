// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Tests;

[TestClass]
public class CQRSTest
{
    private IServiceCollection _services;
    private IServiceProvider _serviceProvider;
    private IEventBus _eventBus;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddEventBus();
        _serviceProvider = _services.BuildServiceProvider();
        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }


    [DataTestMethod]
    [DataRow("")]
    [DataRow("tom")]
    public void TestCommand(string name)
    {
        var command = new CreateProductionCommand()
        {
            Name = name,
            Count = 0
        };
        _eventBus.PublishAsync(command);
        if (string.IsNullOrEmpty(name))
        {
            Assert.IsTrue(command.Count == 2);
        }
        else
        {
            Assert.IsTrue(command.Count == 1);
        }
    }

    [TestMethod]
    public void TestQuery()
    {
        var query = new ProductionItemQuery()
        {
            ProductionId = "1"
        };
        _eventBus.PublishAsync(query);
        Assert.IsTrue(query.Result == "Apple");
    }
}
