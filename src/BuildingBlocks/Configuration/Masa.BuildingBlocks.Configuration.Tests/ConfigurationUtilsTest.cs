// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration.Tests;

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
    [DataRow("masa-dev", "masa-dev")]
    [DataTestMethod]
    public void CompletionParameter(string appId, string expectedAppId)
    {
        var actualAppId = ConfigurationUtils.CompletionParameter(appId, _configuration, null);
        Assert.AreEqual(expectedAppId, actualAppId);
    }
}
