// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests;

[TestClass]
public class CallerTest
{
    private const string DEFAULT_APP_ID = "masa";

    private const string APP_ID_BY_JSON_CONFIG = "masa-appsettings";

    private static FieldInfo AppIdFieldInfo => GetCustomFieldInfo(typeof(DaprCaller), "AppId");

    [TestMethod]
    public void TestAppIdByUseDapr()
    {
        var services = new ServiceCollection();
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(DEFAULT_APP_ID, GetAppId(caller));
    }

    [TestMethod]
    public void TestAppIdByUseDaprAndAlwaysGetNewestDaprClient()
    {
        var services = new ServiceCollection();
        var key = "callerBaseAddress" + Guid.NewGuid();
        Environment.SetEnvironmentVariable(key, DEFAULT_APP_ID);
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseDapr(client =>
            {
                client.AppId = Environment.GetEnvironmentVariable(key)!;
            });
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(DEFAULT_APP_ID, GetAppId(caller));

        var expectedAppId = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(key, expectedAppId);

        callerFactory = serviceProvider.CreateScope().ServiceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(expectedAppId, GetAppId(caller));

        Environment.SetEnvironmentVariable(key, string.Empty);
    }

    [TestMethod]
    public void TestAppIdByUseDaprAndSetEnvironment()
    {
        var services = new ServiceCollection();
        var actualAppId = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(DEFAULT_APP_ID, actualAppId);
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseDapr(client => client.AppId = DEFAULT_APP_ID);
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
    public void TestAppIdByUseDaprAndSetEnvironmentAndUseJsonConfig()
    {
        var services = new ServiceCollection();
        AddJsonConfig(services);

        var actualAppId = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable(DEFAULT_APP_ID, actualAppId);
        services.AddCaller(callerOptions =>
        {
            callerOptions.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(APP_ID_BY_JSON_CONFIG, GetAppId(caller));
        Environment.SetEnvironmentVariable(DEFAULT_APP_ID, string.Empty);
    }

    [TestMethod]
    public void TestAppIdByUseDaprAndUseJsonConfig()
    {
        var services = new ServiceCollection();
        AddJsonConfig(services);

        services.AddCaller(callerOptions =>
        {
            callerOptions.UseDapr(client => client.AppId = DEFAULT_APP_ID);
        });

        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetService<ICallerFactory>();
        Assert.IsNotNull(callerFactory);

        var caller = callerFactory.Create();
        Assert.IsNotNull(caller);

        Assert.AreEqual(APP_ID_BY_JSON_CONFIG, GetAppId(caller));
    }

    [TestMethod]
    public void TestAutoRegistration()
    {
        var services = new ServiceCollection();
        services.AddAutoRegistrationCaller(typeof(CustomDaprCaller).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var customDaprCaller = serviceProvider.GetService<CustomDaprCaller>();
        Assert.IsNotNull(customDaprCaller);

        Assert.AreEqual(DEFAULT_APP_ID, GetAppId(customDaprCaller.GetBaseCaller()));

        var docDaprCaller = serviceProvider.GetService<DocDaprCaller>();
        Assert.IsNotNull(docDaprCaller);

        Assert.AreEqual("doc", GetAppId(docDaprCaller.GetBaseCaller()));
    }

    private static void AddJsonConfig(IServiceCollection services)
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        services.AddSingleton<IConfiguration>(configurationRoot);
    }

    private static string GetAppId(ICaller caller) =>
        (string)AppIdFieldInfo.GetValue(caller)!;

    private static FieldInfo GetCustomFieldInfo(Type type, string name)
        => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
}
