// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore.Tests;

[TestClass]
public class UnitOfWorkManagerTest : TestBase
{
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(masaDbContextBuilder => masaDbContextBuilder.UseSqlite(Connection));
        services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<CustomDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = false,
        });
        services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public void TestUnitOfWorkManager()
    {
        var unitOfWorkManager = new UnitOfWorkManager<CustomDbContext>(_serviceProvider);
        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = _serviceProvider.GetRequiredService<CustomDbContext>();
        var dbContext2 = _serviceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(dbContext.Equals(dbContext2));

        var newUnitOfWork =
            unitOfWorkManager.CreateDbContext(new DbContextConnectionStringOptions(_connectionString));
        Assert.IsFalse(newUnitOfWork.Equals(unitOfWork));
        var newDbContext = newUnitOfWork.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsFalse(dbContext.Equals(newDbContext));

        Assert.ThrowsException<ArgumentException>(() => unitOfWorkManager.CreateDbContext(new DbContextConnectionStringOptions("")));
    }
}
