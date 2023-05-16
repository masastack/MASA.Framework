// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests.Scenes.MasaConfiguration;

[TestClass]
public class CallerTest
{
    private const string DEFAULT_APP_ID = "masa";

    private const string APP_ID_BY_JSON_CONFIG = "masa-appsettings";

    private static FieldInfo AppIdFieldInfo => GetCustomFieldInfo(typeof(DaprCaller), "AppId");

    [TestMethod]
    public void TestAppIdByUseDaprAndSetEnvironmentAndUseJsonConfig()
    {
        var actualAppId = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(DEFAULT_APP_ID, actualAppId);

        var builder = WebApplication.CreateBuilder();
        var services = builder.Services;
        services.AddMasaConfiguration();

        services.AddCaller(callerBuilder =>
        {
            callerBuilder.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(actualAppId, GetAppId(caller));
        Environment.SetEnvironmentVariable(DEFAULT_APP_ID, string.Empty);
    }

    [TestMethod]
    public void TestAppIdByUseDaprAndUseJsonConfig()
    {
        var builder = WebApplication.CreateBuilder();
        var services = builder.Services;
        services.AddMasaConfiguration();

        services.AddCaller(callerBuilder =>
        {
            callerBuilder.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(APP_ID_BY_JSON_CONFIG, GetAppId(caller));
    }

    [TestMethod]
    public void TestAppIdByUseDaprAndUseJsonConfig2()
    {
        var services = new ServiceCollection();
        services.AddMasaConfiguration();

        services.AddCaller(callerBuilder =>
        {
            callerBuilder.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(APP_ID_BY_JSON_CONFIG, GetAppId(caller));
    }

    private static string GetAppId(ICaller caller) =>
        (string)AppIdFieldInfo.GetValue(caller)!;

    private static FieldInfo GetCustomFieldInfo(Type type, string name)
        => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
}
