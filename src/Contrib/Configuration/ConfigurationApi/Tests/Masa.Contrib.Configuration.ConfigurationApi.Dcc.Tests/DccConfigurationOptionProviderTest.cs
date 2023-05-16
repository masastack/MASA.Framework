// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

[TestClass]
public class DccConfigurationOptionProviderTest
{
    private DccConfigurationOptionsCache _dccConfigurationOptionsCache;
    private DccConfigurationOptions _dccConfigurationOptionsByTest;
    private DccConfigurationOptionProvider _dccConfigurationOptionProvider;


    [TestInitialize]
    public void Init()
    {
        _dccConfigurationOptionsCache = new DccConfigurationOptionsCache();
        _dccConfigurationOptionsByTest = new DccConfigurationOptions();
        _dccConfigurationOptionsCache.TryAdd("test", env => _dccConfigurationOptionsByTest);
        _dccConfigurationOptionProvider = new DccConfigurationOptionProvider(_dccConfigurationOptionsCache);
    }

    [TestMethod]
    public void TestGetOptions()
    {
        var services = new ServiceCollection();
        var masaConfigurationEnvironmentCache = new MasaConfigurationEnvironmentCache();
        var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(masaConfigurationEnvironmentCache);
        services.AddSingleton(masaConfigurationEnvironmentProvider);
        var serviceProvider = services.BuildServiceProvider();
        masaConfigurationEnvironmentCache.TryAdd(serviceProvider, _ => "test");

        var dccConfigurationOptions = _dccConfigurationOptionProvider.GetOptions(serviceProvider, null);
        Assert.AreEqual(_dccConfigurationOptionsByTest, dccConfigurationOptions);
    }

    [TestMethod]
    public void TestGetOptionsByNoCache()
    {
        var services = new ServiceCollection();
        services.AddOptions();
        var masaConfigurationEnvironmentCache = new MasaConfigurationEnvironmentCache();
        var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(masaConfigurationEnvironmentCache);
        services.AddSingleton(masaConfigurationEnvironmentProvider);
        var serviceProvider = services.BuildServiceProvider();
        masaConfigurationEnvironmentCache.TryAdd(serviceProvider, _ => "production");

        var dccOptions = new DccOptions()
        {
            ManageServiceAddress = "ManageServiceAddress",
            SubscribeKeyPrefix = "SubscribeKeyPrefix",
            PublicId = "PublicId",
            PublicSecret = "PublicSecret",
            ConfigObjectSecret = "ConfigObjectSecret",
            Environment = "production",
            Cluster = "Cluster",
            AppId = "AppId",
            Secret = "Secret",
            RedisOptions = new RedisConfigurationOptions()
            {
                Servers = new List<RedisServerOptions>()
                {
                    new()
                }
            },
            ConfigObjects = new List<string>()
            {
                "ConnectionStrings"
            },
            ExpandSections = new List<DccSectionOptions>()
            {
                new()
                {
                    AppId = "PublicId",
                    Secret = "Secret",
                    ConfigObjects = new List<string>()
                    {
                        "ConnectionStrings"
                    }
                }
            }
        };

        var dccConfigurationOptions = _dccConfigurationOptionProvider.GetOptions(serviceProvider, sp => dccOptions);
        Assert.AreEqual(dccOptions.PublicId, dccConfigurationOptions.PublicId);
        Assert.AreEqual(dccOptions.PublicSecret, dccConfigurationOptions.PublicSecret);
        Assert.AreEqual(dccOptions.ManageServiceAddress, dccConfigurationOptions.ManageServiceAddress);
        Assert.AreEqual(dccOptions.SubscribeKeyPrefix, dccConfigurationOptions.SubscribeKeyPrefix);
        Assert.AreEqual(dccOptions.ConfigObjectSecret, dccConfigurationOptions.ConfigObjectSecret);
        Assert.AreEqual(dccOptions.Environment, dccConfigurationOptions.DefaultSection.Environment);
        Assert.AreEqual(dccOptions.Cluster, dccConfigurationOptions.DefaultSection.Cluster);
        Assert.AreEqual(dccOptions.AppId, dccConfigurationOptions.DefaultSection.AppId);
        Assert.AreEqual(dccOptions.Secret, dccConfigurationOptions.DefaultSection.Secret);
        Assert.AreEqual(dccOptions.ConfigObjects.Count, dccConfigurationOptions.DefaultSection.ConfigObjects.Count);
        Assert.AreEqual(1, dccConfigurationOptions.ExpandSections.Count);
        Assert.AreEqual(dccOptions.PublicId, dccConfigurationOptions.ExpandSections[0].AppId);
        Assert.AreEqual(dccOptions.Environment, dccConfigurationOptions.ExpandSections[0].Environment);
        Assert.AreEqual(dccOptions.Cluster, dccConfigurationOptions.ExpandSections[0].Cluster);
        Assert.AreEqual(dccOptions.Secret, dccConfigurationOptions.ExpandSections[0].Secret);
        Assert.AreEqual(dccOptions.ConfigObjects.Count, dccConfigurationOptions.ExpandSections[0].ConfigObjects.Count);
    }
}
