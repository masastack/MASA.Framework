// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class ConfigurationUtilsTest
{
    private IConfiguration _configuration;

    [TestInitialize]
    public void Initialize()
    {
        Environment.SetEnvironmentVariable("masa-test", "masa");
        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
    }

    [DataRow("masa-test", "masa")]
    [DataRow("masa-dev", "masa")]
    [DataRow("masa-pro", "masa-pro")]
    [DataTestMethod]
    public void CompletionParameter(string appId, string expectedAppId)
    {
        var services = new ServiceCollection();
        services.AddMasaConfiguration(masaConfigurationBuilder =>
        {
            masaConfigurationBuilder.AddConfiguration(_configuration);
            masaConfigurationBuilder.AddJsonFile("appsettings.json");
        }, options => options.Assemblies = Array.Empty<Assembly>());
        var actualAppId = ConfigurationUtils.CompletionParameter(appId, _configuration, services.GetMasaConfiguration());
        Assert.AreEqual(expectedAppId, actualAppId);
    }
}
