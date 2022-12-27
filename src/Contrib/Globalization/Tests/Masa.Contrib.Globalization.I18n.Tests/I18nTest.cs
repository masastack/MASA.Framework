// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Tests;

[TestClass]
// ReSharper disable once InconsistentNaming
public class I18nTest
{
    private static readonly string DefaultResource = Path.Combine("Resources", "I18n");

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
        // ReSharper disable once InconsistentNaming
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
        builder.Services.TestAddI18n(DefaultResource);
        var serviceProvider = builder.Services.BuildServiceProvider();
        // ReSharper disable once InconsistentNaming
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
        builder.Services.AddI18n(DefaultResource);
        var serviceProvider = builder.Services.BuildServiceProvider();
        // ReSharper disable once InconsistentNaming
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
    [DataRow("zh-CN", "Name", "吉姆", false)]
    [DataRow("en-US", "Name", "Jim", false)]
    [DataRow("zh-CN", "Name2", "吉姆2", true)]
    [DataRow("en-US", "Name2", "Jim2", true)]
    public void TestAddMultiResources(string culture, string key, string expectedResult, bool isCustom)
    {
        var services = new ServiceCollection();
        services.Configure<MasaI18nOptions>(options =>
            options.Resources
                .Add<CustomResource>()
                .AddJson(Path.Combine("Resources", "I18n2")));
        services.AddI18n();
        BuildingBlocks.Globalization.I18n.I18n.SetUiCulture(culture);
        var actualResult = "";
        actualResult = !isCustom ?
            BuildingBlocks.Globalization.I18n.I18n.T(key) :
            services.BuildServiceProvider().GetRequiredService<II18n<CustomResource>>().T(key);
        Assert.AreEqual(expectedResult, actualResult);
    }

    [DataTestMethod]
    [DataRow("zh-CN", "Name", "吉姆", false)]
    [DataRow("zh-CN", "name", "吉姆", false)]
    [DataRow("en-US", "Name", "Jim", false)]
    [DataRow("en-US", "name", "Jim", false)]
    [DataRow("zh-CN", "Name2", "吉姆2", true)]
    [DataRow("zh-CN", "name2", "吉姆2", true)]
    [DataRow("en-US", "Name2", "Name2", true)]
    [DataRow("en-US", "name2", "name2", true)]
    public void TestAddMultiResources2(string culture, string key, string expectedResult, bool isCustom)
    {
        var services = new ServiceCollection();
        services.Configure<MasaI18nOptions>(options =>
            options.Resources
                .Add<CustomResource>()
                .AddJson(Path.Combine("Resources", "I18n2"), new List<CultureModel>()
                {
                    new("zh-CN")
                }));
        services.AddI18n();
        BuildingBlocks.Globalization.I18n.I18n.SetUiCulture(culture);
        var actualResult = "";
        actualResult = !isCustom ?
            BuildingBlocks.Globalization.I18n.I18n.T(key) :
            services.BuildServiceProvider().GetRequiredService<II18n<CustomResource>>().T(key);
        Assert.AreEqual(expectedResult, actualResult);
    }

    [DataTestMethod]
    [DataRow("zh-CN", "Name3", "吉姆3")]
    [DataRow("zh-CN", "name3", "吉姆3")]
    [DataRow("en-US", "Name3", "Jim3")]
    [DataRow("en-US", "name3", "Jim3")]
    public void TestAddMultiResources3(string culture, string key, string expectedResult)
    {
        var services = new ServiceCollection();
        services.AddI18nByEmbedded(AppDomain.CurrentDomain.GetAssemblies(), settings =>
        {
            settings.SupportedCultures = new List<CultureModel>
            {
                new("en-US"),
                new("zh-CN"),
            };
            settings.ResourcesDirectory = Path.Combine("Resources", "I18n3");
        });
        BuildingBlocks.Globalization.I18n.I18n.SetUiCulture(culture);
        var actualResult = BuildingBlocks.Globalization.I18n.I18n.T(key);
        Assert.AreEqual(expectedResult, actualResult);
    }
}
