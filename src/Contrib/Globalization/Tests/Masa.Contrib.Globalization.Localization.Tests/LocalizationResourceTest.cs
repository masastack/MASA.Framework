// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization.Tests;

[TestClass]
public class LocalizationResourceTest
{
    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization(string cultureName, string expectedValue)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TestAddMasaLocalization(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var localizer = serviceProvider.GetRequiredService<IMasaStringLocalizer>();
        var value = localizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = localizer.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = localizer["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalization2(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.TestAddMasaLocalization(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var localizer = serviceProvider.GetRequiredService<IMasaStringLocalizer>();
        var value = localizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = localizer.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = localizer["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-CN", "吉姆")]
    [DataRow("en-US", "Jim")]
    public void TestLocalizationByMasaConfigurationLocal(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration();
        builder.Services.TestAddMasaLocalization(options =>
        {
            options.UseJson("Resources");
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var localizer = serviceProvider.GetRequiredService<IMasaStringLocalizer>();
        var value = localizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = localizer.T("Name");
        Assert.AreEqual(expectedValue, value);
        value = localizer["Name2"];
        Assert.AreEqual("Name2", value);
    }
}
