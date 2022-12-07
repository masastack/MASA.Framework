// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace Masa.Contrib.Service.Caller.Tests;

[TestClass]
public class DefaultCallerProviderTest
{
    [DataTestMethod]
    [DataRow(5000, "test", "appid", "appid-test")]
    public void TestCompletionAppId(int appPort, string appIdSuffix, string appId, string expectedAppId)
    {
        var services = new ServiceCollection();
        services.Configure<DaprOptions>(options =>
        {
            options.AppPort = (ushort)appPort;
            options.AppIdSuffix = appIdSuffix;
        });
        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId2()
    {
        string appId = "appid";
        string expectedAppId = $"{appId}-{NetworkUtils.GetPhysicalAddress()}";
        var services = new ServiceCollection();
        services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
        });
        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId3()
    {
        string appId = "appid";
        string expectedAppId = $"{appId}-{Guid.NewGuid()}";
        var services = new ServiceCollection();
        services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
        });
        Mock<IMasaConfiguration> masaConfiguration = new();
        masaConfiguration.Setup(configuration => configuration.Local.GetSection($"{appId}-{NetworkUtils.GetPhysicalAddress()}").Value)
            .Returns(() => expectedAppId);
        Mock<IConfiguration> configuration = new();

        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, configuration.Object, masaConfiguration.Object);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId4()
    {
        string appId = "appid";
        var services = new ServiceCollection();
        services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });
        string expectedAppId = $"{appId}-{Guid.NewGuid()}";

        Mock<IMasaConfiguration> masaConfiguration = new();
        masaConfiguration.Setup(configuration => configuration.Local.GetSection($"{appId}-suffix").Value)
            .Returns(() => expectedAppId);
        Mock<IConfiguration> configuration = new();

        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, configuration.Object, masaConfiguration.Object);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId5()
    {
        string appId = "appid";
        var services = new ServiceCollection();
        services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });
        string expectedAppId = $"{appId}-{Guid.NewGuid()}";

        Mock<IConfiguration> configuration = new();
        configuration.Setup(c => c.GetSection($"{appId}-suffix").Value).Returns(() => expectedAppId);

        var serviceProvider = services.BuildServiceProvider();
        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, configuration.Object);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }
}
