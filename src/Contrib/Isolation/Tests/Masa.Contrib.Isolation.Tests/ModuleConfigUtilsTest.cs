// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class ModuleConfigUtilsTest
{
    [TestMethod]
    public void TestTryGetConfig()
    {
        var services = new ServiceCollection();
        services.Configure<IsolationOptions>(options =>
        {
            options.SectionName = "Isolation";
        });
        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        services.AddSingleton<IConfiguration>(configurationBuilder.Build());
        var serviceProvider = services.BuildServiceProvider();
        var dbConnectionOptions =
            ModuleConfigUtils.GetModuleConfigs<ConnectionStrings>(serviceProvider, string.Empty, ConnectionStrings.DEFAULT_SECTION);
        Assert.IsNotNull(dbConnectionOptions);
        Assert.AreEqual(1, dbConnectionOptions.Count);
        Assert.AreEqual("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity;",
            dbConnectionOptions[0].Data.DefaultConnection);

        var appConfig = ModuleConfigUtils.GetModuleConfigs<AppConfig>(serviceProvider, string.Empty, nameof(AppConfig));
        Assert.IsNotNull(appConfig);
        Assert.AreEqual(1, appConfig.Count);
        Assert.AreEqual("masa", appConfig[0].Data.Name);

        appConfig = ModuleConfigUtils.GetModuleConfigs<AppConfig>(serviceProvider, string.Empty, $"{nameof(AppConfig)}2");
        Assert.IsNotNull(appConfig);
        Assert.AreEqual(1, appConfig.Count);
        Assert.AreEqual("masa2", appConfig[0].Data.Name);
    }
}
