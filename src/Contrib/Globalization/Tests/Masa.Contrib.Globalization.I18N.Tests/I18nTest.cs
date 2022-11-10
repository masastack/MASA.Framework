// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Tests;

[TestClass]
public class I18NTest
{
    private const string DEFAULT_RESOURCE = "Resources/I18N";

    [TestInitialize]
    public void Initialize()
    {
        I18NResourceResourceConfiguration.Resources = new();
    }

    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization(string cultureName, string expectedValue)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TestAddI18N();
        var serviceProvider = services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetUiCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    // [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization2(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.TestAddI18N(DEFAULT_RESOURCE);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetUiCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-TW", "Name")]
    public void TestLocalization3(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddI18N(DEFAULT_RESOURCE);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetUiCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }
}
