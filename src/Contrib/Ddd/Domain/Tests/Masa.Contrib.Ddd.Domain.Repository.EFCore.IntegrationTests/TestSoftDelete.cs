// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests;

[TestClass]
public class TestSoftDelete
{
    private IServiceProvider _serviceProvider;

    private void Initialize(bool enableSoftDelete = true)
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}")
                .UseFilter(options => options.EnableSoftDelete = enableSoftDelete);
        });
        var dispatcherOptions = new CustomDispatcherOptions(services, AppDomain.CurrentDomain.GetAssemblies());
        dispatcherOptions.UseUoW<CustomDbContext>();
        dispatcherOptions.UseRepository<CustomDbContext>();
        _serviceProvider = services.BuildServiceProvider();

        var customDbContext = _serviceProvider.GetRequiredService<CustomDbContext>();
        customDbContext.Database.EnsureCreated();
        var order = new Orders(1)
        {
            Description = "Description",
            ReceiveAddress = new Address()
            {
                City = "city",
                Country = new Country()
                {
                    Name = "Country Name"
                }
            }
        };
        order.OrderItems.Add(new OrderItem()
        {
            PictureUrl = "PictureUrl",
            ProductName = "ProductName"
        });
        customDbContext.Set<Orders>().Add(order);
        customDbContext.SaveChanges();
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        Initialize();
        await using var scope = _serviceProvider.CreateAsyncScope();
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.AreEqual(1, await customDbContext.Set<Orders>().CountAsync());

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Orders>>();
        var order = await repository.FindAsync(o => o.Id == 1);
        Assert.IsNotNull(order);
        await repository.RemoveAsync(order);

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();

        Assert.AreEqual(0, await customDbContext.Set<Orders>().CountAsync());
    }

    [TestMethod]
    public async Task TestFindAsync()
    {
        Initialize();
        await using var scope = _serviceProvider.CreateAsyncScope();
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        var order = await customDbContext.Set<Orders>().FirstOrDefaultAsync(o => o.Id == 1);
        Assert.IsNotNull(order);

        customDbContext.Set<Orders>().Remove(order);
        await customDbContext.SaveChangesAsync();

        order = await customDbContext.Set<Orders>().FirstOrDefaultAsync(o => o.Id == 1);
        Assert.IsNull(order);
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Orders, int>>();
        order = await repository.FindAsync(o => o.Id == 1);
        Assert.IsNull(order);

        order = await repository.FindAsync(new List<KeyValuePair<string, object>>()
        {
            new("Id", 1)
        });
        Assert.IsNull(order);

        order = await repository.FindAsync(1);
        Assert.IsNull(order);
    }

    [TestMethod]
    public async Task TestFindAsyncByNotUseSoftDelete()
    {
        Initialize(false);
        await using var scope = _serviceProvider.CreateAsyncScope();
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        var order = await customDbContext.Set<Orders>().FirstOrDefaultAsync(o => o.Id == 1);
        Assert.IsNotNull(order);

        customDbContext.Set<Orders>().Remove(order);
        await customDbContext.SaveChangesAsync();

        order = await customDbContext.Set<Orders>().FirstOrDefaultAsync(o => o.Id == 1);
        Assert.IsNull(order);
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Orders, int>>();
        order = await repository.FindAsync(o => o.Id == 1);
        Assert.IsNull(order);

        order = await repository.FindAsync(new List<KeyValuePair<string, object>>()
        {
            new("Id", 1)
        });
        Assert.IsNull(order);

        order = await repository.FindAsync(1);
        Assert.IsNull(order);
    }
}
