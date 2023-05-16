// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests.Scenes.MasaConfiguration;

[TestClass]
public class DefaultCallerProviderTest
{
    private const string APPID = "appid";

    [TestInitialize]
    public void InitializeData()
    {
        Environment.SetEnvironmentVariable($"{APPID}-suffix", "");
    }

    [TestMethod]
    public void TestCompletionAppId()
    {
        string expectedAppId = "expected-appid";
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptions<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration);
        string actualAppId = callerProvider.CompletionAppId(APPID);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId2()
    {
        string expectedAppId = $"{APPID}-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable($"{APPID}-suffix", expectedAppId);
        var builder = WebApplication.CreateBuilder();

        //If the newly added data source exists, the environment variable data source will no longer be queried
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppId = APPID;
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptions<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration);
        string actualAppId = callerProvider.CompletionAppId(APPID);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId3()
    {
        string expectedAppId = "expected-appid";
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration();
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptions<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration, builder.Services.GetMasaConfiguration());
        string actualAppId = callerProvider.CompletionAppId(APPID);
        Assert.AreEqual(expectedAppId, actualAppId);
    }

    [TestMethod]
    public void TestCompletionAppId4()
    {
        string expectedAppId = $"{APPID}-{Guid.NewGuid()}";

        Environment.SetEnvironmentVariable($"{APPID}-suffix", expectedAppId);
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration();
        builder.Services.Configure<DaprOptions>(options =>
        {
            options.AppPort = 5000;
            options.AppIdSuffix = "suffix";
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        var daprOptions = serviceProvider.GetRequiredService<IOptions<DaprOptions>>();

        var callerProvider = new DefaultCallerProvider(daprOptions, builder.Configuration, builder.Services.GetMasaConfiguration());
        string actualAppId = callerProvider.CompletionAppId(APPID);
        Assert.AreEqual(expectedAppId, actualAppId);
    }
}
