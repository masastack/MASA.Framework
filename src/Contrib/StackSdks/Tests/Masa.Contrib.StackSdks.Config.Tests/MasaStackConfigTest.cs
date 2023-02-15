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
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
        var configs = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION) },
            { MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() },
            { MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME) },
            { MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE) },
            { MasaStackConfigConstant.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConstant.TLS_NAME) },
            { MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER) },
            { MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL) },
            { MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS) },
            { MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING) },
            { MasaStackConfigConstant.MASA_SERVER, configuration.GetValue<string>(MasaStackConfigConstant.MASA_SERVER) },
            { MasaStackConfigConstant.MASA_UI, configuration.GetValue<string>(MasaStackConfigConstant.MASA_UI) },
            { MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC) },
            { MasaStackConfigConstant.ENVIRONMENT, configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT) },
            { MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD) },
            { MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET) }
        };

        _stackConfig = new MasaStackConfig(configs, null);
    }

    [TestMethod]
    public void TestGetAllServers()
    {
        var allServer = _stackConfig.GetAllServer();

        Assert.IsNotNull(allServer);
    }

    [TestMethod]
    public void TestGetDefaultDccOptions()
    {
        var dccOptions = _stackConfig.GetDefaultDccOptions();

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
    public void TestGetVersion()
    {
        var version = _stackConfig.Version;

        Assert.IsNotNull(version);
    }

    [TestMethod]
    public void TestGetWebId()
    {
        var pmWebId = _stackConfig.GetWebId(MasaStackConstant.PM);

        Assert.AreEqual("masa-pm-ui-demo", pmWebId);
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
