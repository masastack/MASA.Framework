// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class WebApplicationBuilderTest
{
    [TestMethod]
    public void TestInitializeAppConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        string env = "Development";
        builder.Services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.Environment = env;
        });
        builder.InitializeAppConfiguration();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var masaAppConfigureOptions = serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()!;

        Assert.IsTrue(masaAppConfigureOptions.Value.Length == 3);
        Assert.IsTrue(masaAppConfigureOptions.Value.Environment == env);
        Assert.IsTrue(masaAppConfigureOptions.Value.GetValue(nameof(MasaAppConfigureOptions.Environment)) == env);
    }
}
