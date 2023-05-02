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
}
