// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.EFCore.Tests;

[TestClass]
public class IsolationDbContextTest
{
    [TestMethod]
    public void TestAddMultiMasaDbContextReturnSaveChangeFilterEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()))
            .AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        Assert.AreEqual(2, serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>().Count());
    }
}
