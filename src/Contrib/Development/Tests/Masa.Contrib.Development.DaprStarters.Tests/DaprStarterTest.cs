// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters.Tests;

[TestClass]
public class DaprStarterTest
{
    [TestMethod]
    public void TestAddDaprStarter()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDaprStarterCore();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual("Test", daprOptions.CurrentValue.AppId);
    }

    [TestMethod]
    public void TestAddDaprStarter2()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDaprStarterCore(builder.Configuration.GetSection("DaprOptions2"));
        var serviceProvider = builder.Services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual("Test2", daprOptions.CurrentValue.AppId);
    }

    [TestMethod]
    public void TestAddDaprStarter3()
    {
        var services = new ServiceCollection();
        services.AddDaprStarterCore(options =>
        {
            options.AppId = "TestAddDaprStarter3";
        });
        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual("TestAddDaprStarter3", daprOptions.CurrentValue.AppId);
    }

    [TestMethod]
    public void TestAddDaprStarter4()
    {
        var services = new ServiceCollection();
        services.AddDaprStarterCore();
        var count = services.Count;
        services.AddDaprStarterCore();
        Assert.AreEqual(count, services.Count);
    }
}
