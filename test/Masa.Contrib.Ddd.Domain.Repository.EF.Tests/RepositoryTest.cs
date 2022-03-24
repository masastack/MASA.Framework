namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests;

[TestClass]
public class RepositoryTest : TestBase
{
    private IServiceCollection _services = default!;
    private Assembly[] _assemblies;
    private Mock<IDispatcherOptions> _dispatcherOptions = default!;
    private CustomDbContext _dbContext;
    private UnitOfWork<CustomDbContext> _uoW;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        await InitializeAsync(null);
    }

    public async Task InitializeAsync(Action<IServiceCollection>? action)
    {
        _services = new ServiceCollection();
        _assemblies = new[]
        {
            typeof(BaseRepositoryTest).Assembly
        };
        _dispatcherOptions = new Mock<IDispatcherOptions>();
        _dispatcherOptions.Setup(options => options.Services).Returns(() => _services);
        _dispatcherOptions.Setup(options => options.Assemblies).Returns(() => _assemblies);
        if (action == null)
            _services.AddMasaDbContext<CustomDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        else
            action.Invoke(_services);

        var serviceProvider = _services.BuildServiceProvider();
        _dbContext = _services.BuildServiceProvider().GetRequiredService<CustomDbContext>();
        await _dbContext.Database.EnsureCreatedAsync();
        _uoW = new UnitOfWork<CustomDbContext>(serviceProvider);
        _dispatcherOptions.Object.UseUoW<CustomDbContext>();
    }

    private async Task<IRepository<Orders>> InitDataAsync()
    {
        _dispatcherOptions.Object.UseRepository<CustomDbContext>();

        var serviceProvider = _services.BuildServiceProvider();
        var orders = new List<Orders>
        {
            new(1)
            {
                OrderNumber = 9999999,
                Description = "Apple"
            },
            new(2)
            {
                OrderNumber = 9999999,
                Description = "HuaWei"
            }
        };

        var repository = serviceProvider.GetRequiredService<IRepository<Orders>>();
        await repository.AddRangeAsync(orders);
        await repository.UnitOfWork.SaveChangesAsync();
        return repository;
    }

    [TestMethod]
    public async Task TestAddAsync()
    {
        var repository = await InitDataAsync();

        var orderList = await repository.GetListAsync(order => order.OrderNumber == 9999999);
        Assert.IsNotNull(orderList);
        Assert.IsTrue(orderList.Count() == 2);

        Assert.IsTrue(await repository.GetCountAsync(order => order.Description == "Apple") == 1);
    }

    [TestMethod]
    public async Task TestRemoveAsync()
    {
        var repository = await InitDataAsync();

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
        await repository.RemoveAsync(order => order.Description == "Apple");
        await repository.UnitOfWork.SaveChangesAsync();
        Assert.IsTrue(await repository.GetCountAsync() == 3);

        var order = await repository.FindAsync(
            new List<KeyValuePair<string, object>>
            {
                new(nameof(Orders.Description), "Google")
            });
        await repository.RemoveAsync(order!);
        await repository.UnitOfWork.SaveChangesAsync();
        Assert.IsTrue(await repository.GetCountAsync(order => order.Description == "Google") == 0);
    }

    [TestMethod]
    public async Task TestRemoveRangeAsync()
    {
        var repository = await InitDataAsync();

        Assert.IsTrue(await repository.GetCountAsync() == 2);

        var remainingOrders = await repository.GetListAsync();
        await repository.RemoveRangeAsync(remainingOrders);
        await repository.UnitOfWork.SaveChangesAsync();

        Assert.IsTrue(await repository.GetCountAsync() == 0);
    }

    [TestMethod]
    public async Task TestGetPaginatedListAsync()
    {
        _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        var serviceProvider = _services.BuildServiceProvider();
        var customizeOrderRepository = serviceProvider.GetRequiredService<ICustomizeOrderRepository>();

        var orders = new List<Orders>
        {
            new(1)
            {
                Description = "HuaWei",
                OrderNumber = 20220228
            },
            new(2)
            {
                Description = "Microsoft",
                OrderNumber = 20220227
            },
            new(3)
            {
                Description = "Apple",
                OrderNumber = 20220227
            }
        };
        await customizeOrderRepository.AddRangeAsync(orders);
        await customizeOrderRepository.UnitOfWork.SaveChangesAsync();

        var sorting = new Dictionary<string, bool>(
            new List<KeyValuePair<string, bool>>
            {
                new("OrderNumber", true),
                new("Description", false)
            });
        var list = await customizeOrderRepository.GetPaginatedListAsync(
            0,
            10,
            sorting);
        Assert.IsTrue(list[0].Id == 1);
        Assert.IsTrue(list[1].Id == 3);
        Assert.IsTrue(list[2].Id == 2);

        sorting = new Dictionary<string, bool>(
            new List<KeyValuePair<string, bool>>
            {
                new("OrderNumber", false),
                new("Description", true)
            });
        list = await customizeOrderRepository.GetPaginatedListAsync(
            order => order.Id != 3,
            0,
            10,
            sorting);
        Assert.IsTrue(list[0].Id == 2);
        Assert.IsTrue(list[1].Id == 1);

        list = await customizeOrderRepository.GetPaginatedListAsync(
            order => order.Id != 3,
            0,
            10,
            null);
        Assert.IsTrue(list[0].Id == 1); //If you do not specify a sort value, the database will sort by default
        Assert.IsTrue(list[1].Id == 2);

        list = await customizeOrderRepository.GetPaginatedListAsync(
            0,
            10,
            null);
        Assert.IsTrue(list[0].Id == 1); //If you do not specify a sort value, the database will sort by default
        Assert.IsTrue(list[1].Id == 2);
    }

    [TestMethod]
    public async Task TestTranscationFailedAsync()
    {
        _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        var serviceProvider = _services.BuildServiceProvider();
        var repository = serviceProvider.GetRequiredService<IOrderRepository>();
        var order = new Orders(1)
        {
            OrderNumber = 1
        };
        await repository.AddAsync(order);
        Assert.IsTrue(await repository.GetCountAsync() == 0);
    }

    [TestMethod]
    public async Task TestTranscationSucceededAsync()
    {
        _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        var serviceProvider = _services.BuildServiceProvider();
        var repository = serviceProvider.GetRequiredService<IOrderRepository>();
        var order = new Orders(1)
        {
            OrderNumber = 1,
            Description = "Apple"
        };
        await repository.AddAsync(order);
        Assert.IsTrue(await repository.GetCountAsync() == 1);
    }

    [TestMethod]
    public async Task TestUpdateAsync()
    {
        await InitializeAsync(services =>
            services.AddMasaDbContext<CustomDbContext>(options =>
            {
                options.DbContextOptionsBuilder.UseSqlite(Connection)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }));

        _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        var serviceProvider = _services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();

        var repository = serviceProvider.GetRequiredService<IOrderRepository>();

        var order = new Orders(1)
        {
            OrderNumber = 1,
            Description = "Apple"
        };
        await repository.AddAsync(order, default);
        await repository.UnitOfWork.SaveChangesAsync();
        dbContext.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        order = await repository.FindAsync(order => order.Description == "Apple");
        order!.Description = "Apple Company";
        await repository.UnitOfWork.SaveChangesAsync();

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNotNull(order);

        await repository.UpdateAsync(order);
        await repository.UnitOfWork.SaveChangesAsync();
        dbContext.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        Assert.IsTrue(await repository.GetCountAsync() == 1);

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNotNull(order);

        order.Description = "Apple Company";
        await repository.UpdateRangeAsync(new List<Orders>
            { order });
        await repository.UnitOfWork.SaveChangesAsync();

        dbContext.Entry(order).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        order = await repository.FindAsync(order => order.Description == "Apple");
        Assert.IsNull(order);
    }

    [TestMethod]
    public async Task TestFindAsync()
    {
        await InitDataAsync();
        var repository = _services.BuildServiceProvider()
            .GetRequiredService<IRepository<Orders, int>>();

        var order = await repository.FindAsync(2);
        Assert.IsNotNull(order);
        Assert.IsTrue(order.Description == "HuaWei");
    }

    [TestMethod]
    public void TestCustomizeOrderRepository()
    {
        _dispatcherOptions.Object.UseRepository<CustomDbContext>();

        var serviceProvider = _services.BuildServiceProvider();
        var customizeOrderRepository = serviceProvider.GetService<ICustomizeOrderRepository>();
        Assert.IsNotNull(customizeOrderRepository);
    }

    [TestMethod]
    public async Task TestEntityStateAsync()
    {
        var repository = new Repository<CustomDbContext, Orders>(_dbContext, _uoW);
        Assert.IsTrue(repository.EntityState == Masa.BuildingBlocks.Data.UoW.EntityState.UnChanged);

        await repository.AddAsync(new Orders(9999)
        {
            Description = "HuaWei"
        });
        Assert.IsTrue(repository.EntityState == BuildingBlocks.Data.UoW.EntityState.Changed);
        await repository.SaveChangesAsync();
        Assert.IsTrue(repository.EntityState == BuildingBlocks.Data.UoW.EntityState.UnChanged);
    }

    [TestMethod]
    public async Task TestCommitStateAsync()
    {
        var repository = new Repository<CustomDbContext, Orders>(_dbContext, _uoW);
        Assert.IsTrue(repository.CommitState == CommitState.Commited);

        await repository.AddAsync(new Orders(9999)
        {
            Description = "HuaWei"
        });
        Assert.IsTrue(repository.CommitState == CommitState.UnCommited);
        await repository.SaveChangesAsync();
        Assert.IsTrue(repository.CommitState == CommitState.UnCommited);
        await repository.CommitAsync();
        Assert.IsTrue(repository.CommitState == CommitState.Commited);
    }

    [TestMethod]
    public async Task TestCommitStateAndNotUseTransactionAsync()
    {
        _uoW.UseTransaction = false;
        var repository = new Repository<CustomDbContext, Orders>(_dbContext, _uoW);
        Assert.IsTrue(repository.CommitState == CommitState.Commited);

        await repository.AddAsync(new Orders(9999)
        {
            Description = "HuaWei"
        });
        Assert.IsTrue(repository.CommitState == CommitState.Commited);
        await repository.SaveChangesAsync();
        Assert.IsTrue(repository.CommitState == CommitState.Commited);
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            await repository.CommitAsync();
        });
        Assert.IsTrue(repository.CommitState == CommitState.Commited);
    }

    [TestMethod]
    public void TestNotUseTransaction()
    {
        var repository = new Repository<CustomDbContext, Orders>(_dbContext, _uoW);
        repository.UseTransaction = false;
        Assert.ThrowsException<NotSupportedException>(() => repository.Transaction);
    }

    [TestMethod]
    public async Task TestDbTransactionAsync()
    {
        var dbTransaction = (await _dbContext.Database.BeginTransactionAsync()).GetDbTransaction();
        var repository = new Repository<CustomDbContext, Orders>(_dbContext, _uoW);
        Assert.IsTrue(repository.Transaction.Equals(dbTransaction));
    }

    [TestMethod]
    public async Task TestServiceLifeAsync()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = services.BuildServiceProvider();

        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
            var uow = new UnitOfWork<CustomDbContext>(scope.ServiceProvider);
            var repository = new Repository<CustomDbContext, Orders>(dbContext, uow);
            await repository.AddAsync(new Orders(1)
            {
                Description = "HuaWei"
            });
            await repository.SaveChangesAsync();
            Assert.IsTrue(await repository.GetCountAsync() == 1);
        }
    }
}
