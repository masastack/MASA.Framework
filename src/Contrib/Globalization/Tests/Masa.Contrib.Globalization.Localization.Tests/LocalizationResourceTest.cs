// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization.Tests;

[TestClass]
public class LocalizationResourceTest
{
    [DataTestMethod]
    [DataRow("zh-cn","吉姆")]
    [DataRow("en","Jim")]
    public void TestLocalization(string cultureName, string expectedValue)
    {
        var services = new ServiceCollection();
        services.Configure<MasaLocalizationOptions>(masaLocalizationOptions =>
        {
            masaLocalizationOptions.DefaultCultureName = "en";

            masaLocalizationOptions.Resources
                .Add<DefaultResource>()
                .AddJson("/Resources");
        });
        services.AddLogging();
        services.AddMasaLocalization();
        var serviceProvider = services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var masaStringLocalizer = serviceProvider.GetRequiredService<IMasaStringLocalizer<DefaultResource>>();
        var value = masaStringLocalizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = masaStringLocalizer["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-cn","吉姆")]
    [DataRow("en","Jim")]
    public void TestLocalization2(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<MasaLocalizationOptions>(masaLocalizationOptions =>
        {
            masaLocalizationOptions.DefaultCultureName = "en";

            masaLocalizationOptions.Resources
                .Add<DefaultResource>()
                .AddJson("/Resources");
        });
        builder.Services.AddMasaLocalization();
        var serviceProvider = builder.Services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var masaStringLocalizer = serviceProvider.GetRequiredService<IMasaStringLocalizer<DefaultResource>>();
        var value = masaStringLocalizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = masaStringLocalizer["Name2"];
        Assert.AreEqual("Name2", value);
    }

    [DataTestMethod]
    [DataRow("zh-cn","吉姆")]
    [DataRow("en","Jim")]
    public void TestLocalizationByMasaConfigurationLocal(string cultureName, string expectedValue)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaConfiguration();
        builder.Services.Configure<MasaLocalizationOptions>(masaLocalizationOptions =>
        {
            masaLocalizationOptions.DefaultCultureName = "en";

            masaLocalizationOptions.Resources
                .Add<DefaultResource>()
                .AddJson("/Resources");
        });
        builder.Services.AddMasaLocalization();
        var serviceProvider = builder.Services.BuildServiceProvider();
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
        var masaStringLocalizer = serviceProvider.GetRequiredService<IMasaStringLocalizer<DefaultResource>>();
        var value = masaStringLocalizer["Name"];
        Assert.AreEqual(expectedValue, value);
        value = masaStringLocalizer["Name2"];
        Assert.AreEqual("Name2", value);
    }
}
