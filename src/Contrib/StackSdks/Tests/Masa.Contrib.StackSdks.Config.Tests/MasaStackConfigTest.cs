// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config.Tests;

[TestClass]
public class MasaStackConfigTest
{
    private IMasaStackConfig _stackConfig;

    [TestInitialize]
    public void Initialize()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddMasaStackConfig();
        _stackConfig = builder.Services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }

    [TestMethod]
    public void TestGetAllServers()
    {
        var allServer = _stackConfig.GetAllServer();

        Assert.IsNotNull(allServer);
    }

    [TestMethod]
    public void TestGetMiniDccOptions()
    {
        var dccOptions = _stackConfig.GetDccMiniOptions<DccOptions>();

        Assert.IsNotNull(dccOptions?.RedisOptions);
    }

    [TestMethod]
    public void TestGetEnvironment()
    {
        var environment = _stackConfig.Environment;

        Assert.IsNotNull(environment);
    }
}
