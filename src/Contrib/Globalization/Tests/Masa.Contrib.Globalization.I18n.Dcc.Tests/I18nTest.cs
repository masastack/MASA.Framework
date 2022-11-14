// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Dcc.Tests;

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
    [DataRow("en-US", "JIM")]
    public void Test(string cultureName, string expectedValue)
    {
        var appId = "appid";
        var configObjectPrefix = "Culture";
        var key = "key";
        var services = new ServiceCollection();
        MasaApp.SetServiceCollection(services);
        Mock<IMasaConfiguration> masaConfiguration = new();
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>()
        {
            new($"{configObjectPrefix}.{cultureName}:{key}", expectedValue)
        });
        var configuration = configurationBuilder.Build();
        masaConfiguration.Setup(config => config.ConfigurationApi.Get(appId)).Returns(configuration);
        services.AddSingleton(masaConfiguration.Object);
        services.AddI18n(options =>
        {
            options.ResourcesDirectory = DEFAULT_RESOURCE;
        }, options => options.UseDcc(appId, configObjectPrefix));
        MasaApp.SetServiceCollection(services);

        var serviceProvider = services.BuildServiceProvider();
        var i18n = serviceProvider.GetService<II18n>();
        Assert.IsNotNull(i18n);
        i18n.SetUiCulture(cultureName);
        var value = i18n.T(key);
        Assert.AreEqual(expectedValue, value);
    }
}
