// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests.Scenes.DisableAutoMap;

[TestClass]
public class ServiceCollectionExtensionsTest
{
    [TestMethod]
    public void TestInitializeAppConfiguration()
    {
        var services = new ServiceCollection();
        services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.SetVariable(nameof(MasaAppConfigureOptions.Environment), "Env", "test1");
        });
        services.AddMasaConfiguration(masaConfigurationBuilder =>
        {
            masaConfigurationBuilder.AddJsonFile("appsettings.json", true, true);
        }, Array.Empty<Assembly>());
        var serviceProvider = services.BuildServiceProvider();
        var masaAppConfigureOptions = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()!;

        Assert.IsTrue(masaAppConfigureOptions.Value.Length == 3);
        Assert.IsTrue(masaAppConfigureOptions.Value.Environment == "Test");

        var masaConfiguration = services.GetMasaConfiguration();
        Assert.AreEqual("Test", masaConfiguration.Local["Env"]);
    }

    [TestMethod]
    public void TestInitializeAppConfiguration2()
    {
        var services = new ServiceCollection();
        services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.SetVariable(nameof(MasaAppConfigureOptions.Environment), "Env", "test1");
        });
        services.AddMasaConfiguration();
        var serviceProvider = services.BuildServiceProvider();
        var masaAppConfigureOptions = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()!;

        Assert.IsTrue(masaAppConfigureOptions.Value.Length == 3);
        Assert.IsTrue(masaAppConfigureOptions.Value.Environment == "test1");
    }
}
