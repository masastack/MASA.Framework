namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests;

[TestClass]
public class RepositoryTest : TestBase
{
    private IServiceCollection _services = default!;
    private Assembly[] _assemblies;
    private Mock<IUnitOfWork> _uoW;
    private Mock<IDispatcherOptions> _dispatcherOptions = default!;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _assemblies = new Assembly[1]
        {
            typeof(BaseRepositoryTest).Assembly
        };
        _uoW = new();
        _uoW.Setup(uoW => uoW.UseTransaction).Returns(true);
        _dispatcherOptions = new();
        _dispatcherOptions.Setup(options => options.Services).Returns(() => _services);

    }

    [TestMethod]
    public async Task TestAsync()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>(_assemblies);

        var serviceProvider = _services.BuildServiceProvider();

        _uoW.Setup(u => u.SaveChangesAsync(default)).Callback(() =>
        {
            var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
            dbContext.Database.EnsureCreated();
            dbContext.SaveChanges();
        });
        _uoW.Setup(u => u.CommitAsync(default)).Verifiable();
        var orders = new List<Orders>()
        {
            new Orders(1)
            {
                OrderNumber = 9999999,
                Description = "Apple",
            },
            new Orders(2)
            {
                OrderNumber = 9999999,
                Description = "Apple2",
            }
        };

        var repository = serviceProvider.GetRequiredService<IRepository<Orders>>();
        await repository.AddRangeAsync(orders);
        await repository.UnitOfWork.SaveChangesAsync();

        var orderList = await repository.GetListAsync(order => order.OrderNumber == 9999999, default);
        Assert.IsNotNull(orderList);
        Assert.IsTrue(orderList.Count() == 2);

        Assert.IsTrue((await repository.GetListAsync(order => order.Description == "Apple", default)).Count() == 1);
        Assert.IsTrue(await repository.GetCountAsync(order => order.Description == "Apple", default) == 1);

        var huaweiOrder = await repository.FindAsync(order => order.Description == "Apple2");
        huaweiOrder!.Description = "HuaWei";
        huaweiOrder.OrderNumber = 9999998;
        await repository.UnitOfWork.SaveChangesAsync(default);

        Assert.IsTrue((await repository.GetListAsync(order => order.Description == "Apple", default)).Count() == 1);
        Assert.IsTrue(await repository.GetCountAsync(order => order.Description == "HuaWei", default) == 1);

        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 9999997,
            Description = "Google"
        });
        await repository.AddAsync(new Orders(4)
        {
            OrderNumber = 9999996,
            Description = "Microsoft"
        });

        await repository.RemoveAsync(order => order.Description == "Apple", default);
        await repository.UnitOfWork.SaveChangesAsync(default);

        var list = await repository.GetPaginatedListAsync(0, 10, null, default);

        Assert.IsTrue(list.Count == 3);
        Assert.IsTrue(list[0].Description == "HuaWei");
        Assert.IsTrue(list[1].Description == "Google");
        Assert.IsTrue(list[2].Description == "Microsoft");

        list = await repository.GetPaginatedListAsync(1, 10, null, default);
        Assert.IsTrue(list.Count == 2);
        Assert.IsTrue(list[0].Description == "Google");
        Assert.IsTrue(list[1].Description == "Microsoft");

        list = await repository.GetPaginatedListAsync(order => order.Description != "Google", 0, 10, null, default);
        Assert.IsTrue(list.Count == 2);
        Assert.IsTrue(list[0].Description == "HuaWei");

        var count = await repository.GetCountAsync(default);
        Assert.IsTrue(count == 3);

        var huaWei = await repository.FindAsync(huaweiOrder.Id, huaweiOrder.OrderNumber);
        await repository.RemoveAsync(huaWei!, default);

        await repository.UnitOfWork.SaveChangesAsync(default);
        Assert.IsTrue(await repository.GetCountAsync(default) == 2);

        var remainingOrders = await repository.GetListAsync(default);
        await repository.RemoveRangeAsync(remainingOrders);
        await repository.UnitOfWork.SaveChangesAsync(default);

        Assert.IsTrue(await repository.GetCountAsync(default) == 0);
    }

    [TestMethod]
    public async Task TestTranscationFailedAsync()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>(_assemblies);

        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();
        dbContext.Database.BeginTransaction();

        _uoW.Setup(u => u.SaveChangesAsync(default)).Callback(() =>
        {
            dbContext.SaveChanges();
        });
        _uoW.Setup(u => u.CommitAsync(default)).Callback(() =>
        {
            var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
            dbContext.Database.CurrentTransaction!.Commit();
        });
        _uoW.Setup(u => u.RollbackAsync(default)).Callback(() =>
        {
            var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
            dbContext.Database.CurrentTransaction!.RollbackAsync();
        });
        var repository = serviceProvider.GetRequiredService<IOrderRepository>();

        var order = new Orders()
        {
            OrderNumber = 1,
        };
        await repository.AddAsync(order);
        Assert.IsTrue(await repository.GetCountAsync(default) == 0);
    }

    [TestMethod]
    public async Task TestTranscationSucceededAsync()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>(_assemblies);

        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();

        _uoW.Setup(u => u.SaveChangesAsync(default)).Callback(() =>
        {
            dbContext.SaveChanges();
        });
        _uoW.Setup(u => u.CommitAsync(default)).Callback(() =>
        {
            var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
            dbContext.Database.CurrentTransaction!.Commit();
        });
        _uoW.Setup(u => u.RollbackAsync(default)).Callback(() =>
        {
            var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
            dbContext.Database.CurrentTransaction!.RollbackAsync();
        });
        var repository = serviceProvider.GetRequiredService<IOrderRepository>();

        var order = new Orders(1)
        {
            OrderNumber = 1,
            Description = "Apple"
        };
        await repository.AddAsync(order);
        Assert.IsTrue(await repository.GetCountAsync(default) == 1);
    }

    [TestMethod]
    public async Task TestUpdateAsync()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>(_assemblies);

        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();

        _uoW.Setup(u => u.SaveChangesAsync(default)).Callback(() =>
        {
            dbContext.SaveChanges();
        });
        var repository = serviceProvider.GetRequiredService<IOrderRepository>();

        var order = new Orders(1)
        {
            OrderNumber = 1,
            Description = "Apple"
        };
        await repository.AddAsync(order, default);
        await repository.UnitOfWork.SaveChangesAsync(default);
        dbContext.Entry(order).State = EntityState.Detached;

        order = await repository.FindAsync(order => order.Description == "Apple");
        order!.Description = "Apple Company";
        await repository.UnitOfWork.SaveChangesAsync();

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNotNull(order);

        await repository.UpdateAsync(order, default);
        await repository.UnitOfWork.SaveChangesAsync();
        dbContext.Entry(order).State = EntityState.Detached;
        Assert.IsTrue(await repository.GetCountAsync(default) == 1);

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNotNull(order);

        order.Description = "Apple Company";
        await repository.UpdateRangeAsync(new List<Orders>() { order }, default);
        await repository.UnitOfWork.SaveChangesAsync();

        dbContext.Entry(order).State = EntityState.Detached;

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNull(order);
    }

    [TestMethod]
    public void TestCompositeKeys()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            _dispatcherOptions.Object.UseRepository<CustomDbContext>(typeof(BaseRepositoryTest).Assembly, typeof(Students).Assembly);
        });
    }

    [TestMethod]
    public void TestErrorCompositeKeys()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        Assert.ThrowsException<ArgumentException>(() =>
       {
           _dispatcherOptions.Object.UseRepository<CustomDbContext>(typeof(BaseRepositoryTest).Assembly, typeof(Courses).Assembly);
       });
    }

    [TestMethod]
    public void TestPrivateEntity()
    {
        _services.AddScoped(typeof(IUnitOfWork), serviceProvider => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(_connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>(typeof(BaseRepositoryTest).Assembly, typeof(Hobbies).Assembly);
    }
}
