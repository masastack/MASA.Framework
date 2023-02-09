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
    public void TestGetAdminPwd()
    {
        Assert.IsNotNull(_stackConfig.AdminPwd);
    }

    [TestMethod]
    public void TestGetEnvironment()
    {
        var environment = _stackConfig.Environment;

        Assert.IsNotNull(environment);
    }

    [TestMethod]
    public void TestGetAllUINames()
    {
        var allUIs = _stackConfig.GetAllUINames();

        Assert.IsNotNull(allUIs);
    }

    [TestMethod]
    public void TestGetSsoDomain()
    {
        var ssoDomain = _stackConfig.GetSsoDomain();

        Assert.IsNotNull(ssoDomain);
    }

    [TestMethod]
    public void TestGetAuthServiceDomain()
    {
        var authServiceDomain = _stackConfig.GetAuthServiceDomain();

        Assert.IsNotNull(authServiceDomain);
    }

    [TestMethod]
    public void TestGetElasticModel()
    {
        var esModel = _stackConfig.ElasticModel;

        Assert.IsNotNull(esModel);
    }

    [TestMethod]
    public void TestGetDefaultUserId()
    {
        var userId = _stackConfig.GetDefaultUserId();
        Assert.IsNotNull(userId);

        var userId2 = _stackConfig.GetDefaultUserId();
        Assert.AreEqual(userId, userId2);
    }

    [TestMethod]
    public void TestGetDefaultTeamId()
    {
        var teamId = _stackConfig.GetDefaultTeamId();
        Assert.IsNotNull(teamId);

        var teamId2 = _stackConfig.GetDefaultTeamId();
        Assert.AreEqual(teamId, teamId2);
    }
}
