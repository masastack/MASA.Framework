// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Development.DaprStarters.AspNetCore.Tests;

[TestClass]
public class DaprStarterTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void TestAddDaprStarter()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDaprStarter();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual("Test", daprOptions.CurrentValue.AppId);
    }

    [DataTestMethod]
    [DataRow("AppId")]
    public void TestAddDaprStarter2(string appId)
    {
        _services.AddDaprStarter(options =>
        {
            options.AppId = appId;
        });
        var serviceProvider = _services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual(appId, daprOptions.CurrentValue.AppId);
    }

    [TestMethod]
    public void TestAddDaprStarter3()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDaprStarter(builder.Configuration.GetSection("DaprOptions2"));
        var serviceProvider = builder.Services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetService<IOptionsMonitor<DaprOptions>>();
        Assert.IsNotNull(daprOptions);
        Assert.IsNotNull(daprOptions.CurrentValue);
        Assert.AreEqual("Test2", daprOptions.CurrentValue.AppId);
    }

    [TestMethod]
    public void TestAddDaprStarter4()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _services.AddDaprStarter(isDelay: false));
    }

    [TestMethod]
    public void TestAddDaprStarter5()
    {
        _services.AddDaprStarter();
        var count = _services.Count;
        _services.AddDaprStarter();
        Assert.AreEqual(count, _services.Count);
    }
}
