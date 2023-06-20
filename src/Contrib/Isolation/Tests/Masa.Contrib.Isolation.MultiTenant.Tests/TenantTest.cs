// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant.Tests;

[TestClass]
public class TenantTest
{
    [TestMethod]
    public void TestSetTenant()
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
        isolationBuilder.Object.UseMultiTenant();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetRequiredService<IMultiTenantContext>().CurrentTenant == null);

        var tenant = new Tenant("1");
        serviceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(tenant);
        Assert.IsTrue(serviceProvider.GetRequiredService<IMultiTenantContext>().CurrentTenant == tenant);
    }

    [DataRow("tenant", "tenant", true)]
    [DataRow("", IsolationConstant.DEFAULT_MULTI_TENANT_NAME, true)]
    [DataRow(null, IsolationConstant.DEFAULT_MULTI_TENANT_NAME, true)]
    [DataTestMethod]
    public void TestSetTenantByAssignName(
        string? inputTenantIdName,
        string expectedTenantIdName,
        bool expectedEnableMultiTenant)
    {
        var services = new ServiceCollection();
        Mock<IIsolationBuilder> isolationBuilder = new();
        isolationBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
        isolationBuilder.Object.UseMultiTenant(inputTenantIdName);

        var serviceProvider = services.BuildServiceProvider();
        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        Assert.AreEqual(expectedTenantIdName, isolationOptions.Value.MultiTenantIdName);
        Assert.AreEqual(expectedEnableMultiTenant, isolationOptions.Value.EnableMultiTenant);
        Assert.AreEqual(false, isolationOptions.Value.EnableMultiEnvironment);
    }
}
