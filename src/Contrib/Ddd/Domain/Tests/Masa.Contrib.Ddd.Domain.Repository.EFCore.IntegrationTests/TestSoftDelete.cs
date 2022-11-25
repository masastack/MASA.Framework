// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.IntegrationTests;

[TestClass]
public class TestSoftDelete
{
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseFilter();
        });
        var dispatcherOptions = new CustomerDispatcherOptions(services, AppDomain.CurrentDomain.GetAssemblies());
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
    public async Task Test()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.AreEqual(1, await customDbContext.Set<Orders>().CountAsync());

        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Orders>>();
        var order = await repository.FindAsync(o => o.Id == 1);

        await repository.RemoveAsync(order);
        await repository.UnitOfWork.SaveChangesAsync();
        await repository.UnitOfWork.CommitAsync();

        Assert.AreEqual(0, await customDbContext.Set<Orders>().CountAsync());
    }
}
