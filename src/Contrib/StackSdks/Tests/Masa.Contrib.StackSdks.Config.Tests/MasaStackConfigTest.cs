// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Configuration;
using Moq;

namespace Masa.Contrib.StackSdks.Config.Tests;

[TestClass]
public class MasaStackConfigTest
{
    private MasaStackConfig _stackConfig;
    private Dictionary<string, string> _config;

    [TestInitialize]
    public void Initialize()
    {
        var builder = WebApplication.CreateBuilder();
        var configuration = builder.Configuration.AddJsonFile("appsettings.json", true, true).Build();
        _config = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION) },
            { MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() },
            { MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME) },
            { MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE) },
            { MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER) },
            { MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL) },
            { MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS) },
            { MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING) },
            { MasaStackConfigConstant.MASA_SERVER, configuration.GetValue<string>(MasaStackConfigConstant.MASA_SERVER) },
            { MasaStackConfigConstant.MASA_STACK, configuration.GetValue<string>(MasaStackConfigConstant.MASA_STACK) },
            { MasaStackConfigConstant.MASA_UI, configuration.GetValue<string>(MasaStackConfigConstant.MASA_UI) },
            { MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC) },
            { MasaStackConfigConstant.ENVIRONMENT, configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT) },
            { MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD) },
            { MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET) },
            { MasaStackConfigConstant.SUFFIX_IDENTITY, configuration.GetValue<string>(MasaStackConfigConstant.SUFFIX_IDENTITY) }
        };

        Mock<IConfigurationApiClient> dccClient = new();

        dccClient.Setup(client => client.GetAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Action<Dictionary<string, string>>>()!))
            .ReturnsAsync(_config);

        _stackConfig = new MasaStackConfig(dccClient.Object, _config);
    }

    [TestMethod]
    public void TestGetAllServers()
    {
        var allServer = _stackConfig.GetAllService();

        Assert.IsNotNull(allServer);
    }

    [TestMethod]
    public void TestGetDefaultDccOptions()
    {
        var dccOptions1 = MasaStackConfigUtils.GetDefaultDccOptions(_config);

        Assert.IsNotNull(dccOptions1?.RedisOptions);
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
    public void TestGetPmDomain()
    {
        var pmDomain = _stackConfig.GetPmServiceDomain();

        Assert.IsNotNull(pmDomain);
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
    public void TestGetServiceId()
    {
        var pmServiceId = _stackConfig.GetServiceId(MasaStackConstant.PM);

        Assert.AreEqual("pm-service", pmServiceId);
    }

    [TestMethod]
    public void TestGetWebId()
    {
        var pmWebId = _stackConfig.GetWebId(MasaStackConstant.PM);

        Assert.AreEqual("pm-web", pmWebId);
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

    [TestMethod]
    public void TestGetEsModel()
    {
        var es = _stackConfig.ElasticModel;

        Assert.IsTrue(es is not null && es.Nodes.Any());
    }

    [TestMethod]
    public void TestGetAuthConnectionString()
    {
        var suffixIdentity = _stackConfig.SuffixIdentity;

        Assert.AreEqual("dev", suffixIdentity);
    }

    [TestMethod]
    public void TestHasAlert()
    {
        var result = _stackConfig.HasAlert();

        Assert.AreEqual(true, result);
    }
}
