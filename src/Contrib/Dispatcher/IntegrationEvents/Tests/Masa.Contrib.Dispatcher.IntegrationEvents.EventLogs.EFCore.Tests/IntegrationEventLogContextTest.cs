// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests;

[TestClass]
public class IntegrationEventLogContextTest : TestBase
{
    [TestMethod]
    public void TestCreateDbContext()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(builder => builder.UseSqlite(ConnectionString));
        var distributedDispatcherOptions = CreateIntegrationEventOptions(services);
        distributedDispatcherOptions.UseEventLog<CustomDbContext>();
        var serviceProvider = services.BuildServiceProvider();

        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var entity = customDbContext.Model.GetEntityTypes()
            .FirstOrDefault(entityType => entityType.Name == typeof(IntegrationEventLog).FullName)!;

        Assert.IsTrue(entity.GetTableName() == "IntegrationEventLog");
        var properties = entity.GetProperties().ToList();
        Assert.IsTrue(properties.Where(x => x.Name == "Id").Select(x => x.IsPrimaryKey()).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "Id").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "Content").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "CreationTime").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "State").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "TimesSent").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "EventTypeName").Select(x => x.IsNullable).FirstOrDefault());

        var integrationEventLogDbContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        Assert.AreEqual(customDbContext, integrationEventLogDbContext.DbContext);
    }

    [TestMethod]
    public void TestAddDbContext()
    {
        var services = new ServiceCollection();
        services.AddDbContext<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = services.BuildServiceProvider();

        var dbContext = serviceProvider.GetService<IntegrationEventLogContext>();
        Assert.IsTrue(dbContext == null);

        Assert.ThrowsExactly<InvalidOperationException>(() => serviceProvider.GetService<CustomDbContext>());
    }

    [TestMethod]
    public void TestUseEventLog()
    {
        var distributedDispatcherOptions = CreateIntegrationEventOptions(new ServiceCollection());
        distributedDispatcherOptions.Services.AddDbContext<CustomDbContext>(options => options.UseSqlite(Connection));
        distributedDispatcherOptions.UseEventLog<CustomDbContext>();
        var serviceProvider = distributedDispatcherOptions.Services.BuildServiceProvider();

        Assert.ThrowsExactly<InvalidOperationException>(() => serviceProvider.GetService<IntegrationEventLogContext>());

        Assert.ThrowsExactly<InvalidOperationException>(() => serviceProvider.GetService<CustomDbContext>());
    }
}
