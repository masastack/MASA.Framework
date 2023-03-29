// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Scenes.Isolation.Tests;

[TestClass]
public class IsolationDbContextTest
{
    [TestMethod]
    public void TestAddIsolationDbContextReturnSaveChangeFilterEqual3()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        Assert.AreEqual(3, serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>().Count());
    }
}
