// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class ConfigurationApiBaseTest
{
    private DccConfigurationOptions _dccConfigurationOptions;

    [TestInitialize]
    public void Initialize()
    {
        _dccConfigurationOptions = new()
        {
            DefaultSection = new DccSectionOptions()
            {
                Environment = "Test",
                Cluster = "Default",
                AppId = "DccTest",
                ConfigObjects = new List<string>()
                {
                    "Test1"
                },
                Secret = "Secret"
            }
        };
    }

    [DataTestMethod]
    [DataRow("DccTest", "Secret")]
    [DataRow("DccTest2", "Secret2")]
    [DataRow("DccTest3", "")]
    public void TestGetSecret(string appId, string secret)
    {
        var api = new CustomConfigurationApi(_dccConfigurationOptions.DefaultSection, new List<DccSectionOptions>()
        {
            new()
            {
                Environment = "Test2",
                Cluster = "Default2",
                AppId = "DccTest2",
                ConfigObjects = new List<string>()
                {
                    "Test12"
                },
                Secret = "Secret2"
            }
        });
        if (string.IsNullOrEmpty(secret))
            Assert.ThrowsException<ArgumentNullException>(() => api.GetSecretByTest(appId));
        else
            Assert.IsTrue(api.GetSecretByTest(appId) == secret);
    }

    [DataTestMethod]
    [DataRow("Test2", "Test2")]
    [DataRow("", "Test")]
    public void TestGetEnvironment(string environment, string outEnvironment)
    {
        var api = new CustomConfigurationApi(_dccConfigurationOptions.DefaultSection, null);
        Assert.IsTrue(api.GetEnvironmentByTest(environment) == outEnvironment);
    }

    [DataTestMethod]
    [DataRow("CustomCluster", "CustomCluster")]
    [DataRow("", "Default")]
    public void GetCluster(string cluster, string outCluster)
    {
        var api = new CustomConfigurationApi(_dccConfigurationOptions.DefaultSection, null);
        Assert.IsTrue(api.GetClusterByTest(cluster) == outCluster);
    }

    [DataTestMethod]
    [DataRow("CustomAppid", "CustomAppid")]
    [DataRow("", "DccTest")]
    public void GetAppid(string appId, string outAppid)
    {
        var api = new CustomConfigurationApi(_dccConfigurationOptions.DefaultSection, null);
        Assert.IsTrue(api.GetAppIdByTest(appId) == outAppid);
    }

    [DataTestMethod]
    [DataRow("configObject", "configObject")]
    [DataRow("", "")]
    public void GetConfigObject(string configObject, string outConfigObject)
    {
        var api = new CustomConfigurationApi(_dccConfigurationOptions.DefaultSection, null);
        if (string.IsNullOrEmpty(configObject))
            Assert.ThrowsException<ArgumentNullException>(() => api.GetConfigObjectByTest(configObject));
        else
            Assert.IsTrue(api.GetConfigObjectByTest(configObject) == outConfigObject);
    }
}
