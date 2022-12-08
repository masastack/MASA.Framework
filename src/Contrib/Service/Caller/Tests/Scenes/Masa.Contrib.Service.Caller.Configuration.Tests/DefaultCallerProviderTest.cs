// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Configuration.Tests;

[TestClass]
public class DefaultCallerProviderTest
{
    [TestMethod]
    public void TestCompletionAppId()
    {
        string appId = "appid";
        string expectedAppId = "expected-appid";
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddJsonFile("customAppsettings.json");
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId2()
    {
        string appId = "appid";
        string expectedAppId = $"{appId}-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable($"{appId}-suffix", expectedAppId);
        var builder = WebApplication.CreateBuilder();

        //If the newly added data source exists, the environment variable data source will no longer be queried
        builder.Configuration.AddJsonFile("customAppsettings.json");
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration);
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual("expected-appid", actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId3()
    {
        string appId = "appid";
        string expectedAppId = "expected-appid";
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(options => options.AddJsonFile("customAppsettings.json"));
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration, builder.Services.GetMasaConfiguration());
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId4()
    {
        string appId = "appid";
        string expectedAppId = $"{appId}-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable($"{appId}-suffix", expectedAppId);
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration(options => options.AddJsonFile("customAppsettings.json"));
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration, builder.Services.GetMasaConfiguration());
        string actualAppId = callerProvider.CompletionAppId(appId);
        Assert.AreEqual(expectedAppId, actualAppId);
    }
}
