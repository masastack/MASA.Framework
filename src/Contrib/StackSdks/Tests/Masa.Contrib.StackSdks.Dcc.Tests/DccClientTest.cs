// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Dcc.Tests;

[TestClass]
public class DccClientTest
{
    [TestMethod]
    public void TestAddDccClient()
    {
        var services = new ServiceCollection();

        services.AddDccClient();

        var dccClient = services.BuildServiceProvider().GetRequiredService<IDccClient>();
        Assert.IsNotNull(dccClient);
    }

    [TestMethod]
    public void TestAddDccClient2()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddDccClient();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();
        var redisOptions = options.Get("masa.contrib.basicability.dcc");
        Assert.AreEqual(1, redisOptions.Servers.Count);
        Assert.AreEqual("localhost", redisOptions.Servers[0].Host);
        Assert.AreEqual(8888, redisOptions.Servers[0].Port);
        Assert.AreEqual(0, redisOptions.DefaultDatabase);

        var dccClient = serviceProvider.GetRequiredService<IDccClient>();
        Assert.IsNotNull(dccClient);
    }

    [TestMethod]
    public void TestAddDccClient3()
    {
        var services = new ServiceCollection();

        services.AddDccClient(options =>
        {
            options.Servers.Add(new RedisServerOptions("localhost", 8888));
            options.DefaultDatabase = 0;
            options.Password = "xxxx";
        });

        var dccClient = services.BuildServiceProvider().GetRequiredService<IDccClient>();
        Assert.IsNotNull(dccClient);
    }
}
