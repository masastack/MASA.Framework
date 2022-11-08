// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.Blazor.Tests;

[TestClass]
public class ServicesCollectionsTest
{
    [TestMethod]
    public void TestAddI18NForBlazor()
    {
        var services = new ServiceCollection();
        services.AddI18NForBlazorServer();
        var descriptor = ServiceDescriptor.Transient(typeof(II18N<>), typeof(I18NOfT<>));
        Assert.IsTrue(services.Any(d
            => d.ServiceType == descriptor.ServiceType && d.Lifetime == descriptor.Lifetime &&
            d.ImplementationType == descriptor.ImplementationType));
    }
}
