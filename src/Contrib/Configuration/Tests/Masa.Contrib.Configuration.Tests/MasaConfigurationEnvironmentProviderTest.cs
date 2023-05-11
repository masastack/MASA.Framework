// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.Tests;

[TestClass]
public class MasaConfigurationEnvironmentProviderTest
{
    [DataRow(false, true, "test", "", "test")]
    [DataRow(false, true, "test", "dev", "test")]
    [DataRow(true, false, "test", "", "")]
    [DataRow(true, true, "test", "dev", "dev")]
    [DataTestMethod]
    public void TestTryGetDefaultEnvironment(
        bool enableMultiEnvironment,
        bool expectedResult,
        string globalEnvironment,
        string environment,
        string expectedEnvironment)
    {
        var services = new ServiceCollection();
        services.Configure<MasaAppConfigureOptions>(options =>
        {
            options.Environment = globalEnvironment;
        });
        if (enableMultiEnvironment)
        {
            services.Configure<IsolationOptions>(options =>
            {
                options.MultiEnvironmentName = "env";
            });
            Mock<IMultiEnvironmentContext> multiEnvironmentContext = new();
            multiEnvironmentContext.Setup(context => context.CurrentEnvironment).Returns(environment);
            services.AddSingleton<IMultiEnvironmentContext>(_ => multiEnvironmentContext.Object);
        }
        var rootServiceProvider = services.BuildServiceProvider();
        var masaConfigurationEnvironmentProvider = new MasaConfigurationEnvironmentProvider(new MasaConfigurationEnvironmentCache());

        var result = masaConfigurationEnvironmentProvider.TryGetDefaultEnvironment(rootServiceProvider.CreateScope().ServiceProvider,
            out string? env);
        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(expectedEnvironment, env);
    }
}
