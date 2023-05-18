// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiEnvironment.Tests;

[TestClass]
public class EnvironmentTest
{
    [TestMethod]
    public void TestSetEnvironment()
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
        isolationBuilder.Object.UseMultiEnvironment();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(string.IsNullOrEmpty(serviceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment));

        serviceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("dev");
        Assert.IsTrue(serviceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment == "dev");
    }

    [TestMethod]
    public void TestUseMultiEnvironment()
    {
        IServiceCollection services = new ServiceCollection();
        Mock<IIsolationBuilder> options = new();
        options.Setup(option => option.Services).Returns(services).Verifiable();
        options.Object.UseMultiEnvironment();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetService<IIsolationMiddleware>() != null);
        Assert.IsTrue(serviceProvider.GetServices<IIsolationMiddleware>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetService<MultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentSetter>() != null);
    }

    [TestMethod]
    public void TestUseMultiEnvironment2()
    {
        IServiceCollection services = new ServiceCollection();
        Mock<IIsolationBuilder> options = new();
        options.Setup(option => option.Services).Returns(services).Verifiable();
        options.Object.UseMultiEnvironment().UseMultiEnvironment();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetService<IIsolationMiddleware>() != null);
        Assert.IsTrue(serviceProvider.GetServices<IIsolationMiddleware>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetService<MultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentSetter>() != null);
    }

    [TestMethod]
    public void TestUseMultiEnvironment3()
    {
        IServiceCollection services = new ServiceCollection();
        Mock<IIsolationBuilder> options = new();
        options.Setup(option => option.Services).Returns(services).Verifiable();
        options.Object.UseMultiEnvironment(new List<IParserProvider>() { });

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetService<IIsolationMiddleware>() != null);
        Assert.IsTrue(serviceProvider.GetServices<IIsolationMiddleware>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetService<MultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentContext>() != null);
        Assert.IsTrue(serviceProvider.GetService<IMultiEnvironmentSetter>() != null);
    }

    [DataRow("env", "env", true)]
    [DataRow("", IsolationConstant.DEFAULT_MULTI_ENVIRONMENT_NAME, true)]
    [DataRow(null, IsolationConstant.DEFAULT_MULTI_ENVIRONMENT_NAME, true)]
    [DataTestMethod]
    public void TestSetEnvironmentByAssignName(
        string? inputEnvironmentName,
        string expectedEnvironmentName,
        bool expectedEnableMultiEnvironment)
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
        isolationBuilder.Object.UseMultiEnvironment(inputEnvironmentName);

        var serviceProvider = services.BuildServiceProvider();
        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        Assert.AreEqual(expectedEnvironmentName, isolationOptions.Value.MultiEnvironmentName);
        Assert.AreEqual(expectedEnableMultiEnvironment, isolationOptions.Value.EnableMultiEnvironment);
        Assert.AreEqual(false, isolationOptions.Value.EnableMultiTenant);
    }
}
