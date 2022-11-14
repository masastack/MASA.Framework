// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Tests;

[TestClass]
public class I18nTest
{
    private const string DEFAULT_RESOURCE = "Resources/I18n";

    [TestInitialize]
    public void Initialize()
    {
        I18nResourceResourceConfiguration.Resources = new();
    }

    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization(string cultureName, string expectedValue)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TestAddI18n();
        var serviceProvider = services.BuildServiceProvider();
        var i18n = serviceProvider.GetRequiredService<II18n>();
        i18n.SetUiCulture(cultureName);
        var value = i18n["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18n.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18n["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    // [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization2(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.TestAddI18n(DEFAULT_RESOURCE);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18n = serviceProvider.GetRequiredService<II18n>();
        i18n.SetUiCulture(cultureName);
        var value = i18n["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18n.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18n["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-TW", "Name")]
    public void TestLocalization3(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddI18n(DEFAULT_RESOURCE);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18n = serviceProvider.GetRequiredService<II18n>();
        i18n.SetUiCulture(cultureName);
        var value = i18n["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18n.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18n["Name2"];
        Assert.AreEqual("Name2", value);
    }
}
