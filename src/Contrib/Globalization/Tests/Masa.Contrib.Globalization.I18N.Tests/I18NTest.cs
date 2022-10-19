// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Tests;

[TestClass]
public class I18NTest
{
    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization(string cultureName, string expectedValue)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TestAddI18N(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization2(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.TestAddI18N(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetCulture(cultureName);
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
        builder.Services.AddI18N(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-TW", "Name")]
    public void TestLocalization4(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<MasaI18NOptions>(options =>
        {
            options.Resources
                .Add<DefaultResource>()
                .AddJson("/Resources");
        });
        builder.Services.AddI18N(null);
        var serviceProvider = builder.Services.BuildServiceProvider();
        var i18N = serviceProvider.GetRequiredService<II18N>();
        i18N.SetCulture(cultureName);
        var value = i18N["Name"];
        Assert.AreEqual(expectedValue, value);
        value = i18N.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = i18N["Name2"];
        Assert.AreEqual("Name2", value);
    }
}
