// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Masa.Contrib.Data.EFCore.Scenes.Isolation.Tests;

[TestClass]
public class MasaDbContextTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        _services.AddSingleton<IConfiguration>(configuration);
    }

    [TestMethod]
    public async Task TestTenantIdByAddEntityAsync()
    {
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
        _services.Configure<IsolationOptions>(options => options.MultiTenantIdType = typeof(int));
        _services.AddIsolation(isolationBuilder => isolationBuilder.UseMultiTenant());
        var rootServiceProvider = _services.BuildServiceProvider();
        var dbContext = rootServiceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = rootServiceProvider.CreateScope();
        var multiTenantSetter = scope.ServiceProvider.GetRequiredService<IMultiTenantSetter>();
        var tenantId = "1";
        multiTenantSetter.SetTenant(new Tenant(tenantId));
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = "masa",
        };
        await customDbContext.Set<User>().AddAsync(user);
        await customDbContext.SaveChangesAsync();

        var userTemp = await customDbContext.User.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.IsNotNull(userTemp);
        Assert.AreEqual(tenantId, userTemp.TenantId.ToString());
    }


    [TestMethod]
    public async Task TestTenantIdByAddOrderAsync()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext2>(dbContext =>
        {
            dbContext.UseInMemoryDatabase(Guid.NewGuid().ToString());
            dbContext.UseFilter();
        });
        services.AddIsolation(isolationBuilder => isolationBuilder.UseMultiTenant());
        var rootServiceProvider = services.BuildServiceProvider();

        var dbContext = rootServiceProvider.GetRequiredService<CustomDbContext2>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = rootServiceProvider.CreateScope();
        var multiTenantSetter = scope.ServiceProvider.GetRequiredService<IMultiTenantSetter>();
        var tenantId = Guid.NewGuid();
        multiTenantSetter.SetTenant(new Tenant(tenantId));

        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext2>();
        var order = new Order()
        {
            Id = Guid.NewGuid(),
        };
        await customDbContext.Set<Order>().AddAsync(order);
        await customDbContext.SaveChangesAsync();

        var tenantId2 = Guid.NewGuid();
        multiTenantSetter.SetTenant(new Tenant(tenantId2));
        var order2 = new Order()
        {
            Id = Guid.NewGuid(),
        };
        await customDbContext.Set<Order>().AddAsync(order2);
        await customDbContext.SaveChangesAsync();

        var orderTemp = await customDbContext.Order.ToListAsync();
        Assert.IsNotNull(orderTemp);

        Assert.AreEqual(1, orderTemp.Count);
        Assert.AreEqual(order2.Id, orderTemp[0].Id);
        Assert.AreEqual(tenantId2, orderTemp[0].TenantId);

        var orderAll = await customDbContext.Order.IgnoreQueryFilters().ToListAsync();
        Assert.AreEqual(2, orderAll.Count);
    }

    [TestMethod]
    public async Task TestTenantIdByAddOrderAndNoConstructorAsync()
    {
        _services.AddMasaDbContext<CustomDbContext3>(builder => builder.UseFilter());
        var rootServiceProvider = _services.BuildServiceProvider();

        var dbContext = rootServiceProvider.GetRequiredService<CustomDbContext3>();
        await dbContext.Database.EnsureCreatedAsync();

        var list = dbContext.Set<Order2>().ToList();
        dbContext.Set<Order2>().RemoveRange(list);
        await dbContext.SaveChangesAsync();

        using var scope = rootServiceProvider.CreateScope();
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext3>();
        var order = new Order2()
        {
            Id = Guid.NewGuid(),
        };
        await customDbContext.Set<Order2>().AddAsync(order);
        await customDbContext.SaveChangesAsync();

        var order2 = new Order2()
        {
            Id = Guid.NewGuid(),
        };
        await customDbContext.Set<Order2>().AddAsync(order2);
        await customDbContext.SaveChangesAsync();

        var orderList = await customDbContext.Order.ToListAsync();
        Assert.IsNotNull(orderList);

        Assert.AreEqual(2, orderList.Count);
        Assert.AreEqual(order.Id, orderList[0].Id);
        Assert.AreEqual(order2.Id, orderList[1].Id);
    }

    [TestMethod]
    public void TestAddMasaDbContextWhenNotUseDatabase()
    {
        _services.AddMasaDbContext<CustomDbContext4>(builder => builder.UseFilter());
        var rootServiceProvider = _services.BuildServiceProvider();

        Assert.ThrowsException<InvalidOperationException>(() => rootServiceProvider.GetRequiredService<CustomDbContext4>());
    }
}
