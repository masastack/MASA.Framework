// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Tests;

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
        var services = new ServiceCollection();
        var options = AppSettings.GetModel<RedisConfigurationOptions>("DccOptions:RedisOptions");

        services.AddDccClient(options);

        var dccClient = services.BuildServiceProvider().GetRequiredService<IDccClient>();
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
