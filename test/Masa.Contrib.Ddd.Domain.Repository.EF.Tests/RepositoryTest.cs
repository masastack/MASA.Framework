// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
            _services.AddMasaDbContext<CustomDbContext>(options => options.UseTestSqlite(Connection));
        else
            action.Invoke(_services);

        var serviceProvider = _services.BuildServiceProvider();
        _dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
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

    private async Task<IRepository<Orders, int>> InitDataAndReturnRepositoryAsync()
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

        var repository = serviceProvider.GetRequiredService<IRepository<Orders, int>>();
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
            10);
        Assert.IsTrue(list[0].Id == 1); //If you do not specify a sort value, the database will sort by default
        Assert.IsTrue(list[1].Id == 2);

        list = await customizeOrderRepository.GetPaginatedListAsync(0, 10);
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
                options.Builder = (_, dbContextOptionsBuilder)
                    => dbContextOptionsBuilder.UseSqlite(Connection)
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
        services.AddMasaDbContext<CustomDbContext>(options => options.UseTestSqlite(Connection));
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

    [TestMethod]
    public async Task TestGetPaginatedListByAscAsyncReturnFirstOrderIdIs1()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(0, 2, "Id", false);
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 1);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByDescAsyncReturnFirstOrderIdIs3()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(0, 2, "Id");
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 3);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByIdGreatherThan1AndAscAsyncReturnFirstOrderIdIs2()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(o => o.Id > 1, 0, 2, "Id", false);
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 2);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByIdGreatherThan1AndDescAsyncReturnFirstOrderIdIs3()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(o => o.Id > 1, 0, 2, "Id");
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 3);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByOptionsAsyncReturnFirstOrderIdIs1()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(new PaginatedOptions(1, 1, "Id", false));
        Assert.IsTrue(paginatedList is { Total: 3 } && paginatedList.Result.First().Id == 1);
    }


    [TestMethod]
    public async Task TestGetPaginatedListByOptionsAndDescAsyncReturnFirstOrderIdIs2()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(new PaginatedOptions(2, 1, "Id", true));
        Assert.IsTrue(paginatedList is { Total: 3 } && paginatedList.Result.First().Id == 2);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByIdGreatherThen2AsyncReturnFirstOrderIdIs2AndCountIs2()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = await repository.GetPaginatedListAsync(o => o.Id > 1, new PaginatedOptions(2, 1, "Id"));
        Assert.IsTrue(paginatedList is { Total: 2 } && paginatedList.Result.First().Id == 2);
    }

    [TestMethod]
    public async Task TestGetListByDescAsyncReturnFirstOrderIdIs1()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = (await repository.GetListAsync("Id", false)).ToList();
        Assert.IsTrue(paginatedList.Count == 3 && paginatedList.First().Id == 1);
    }

    [TestMethod]
    public async Task TestGetListByAscAsyncReturnFirstOrderIdIs3()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = (await repository.GetListAsync("Id")).ToList();
        Assert.IsTrue(paginatedList.Count == 3 && paginatedList.First().Id == 3);
    }

    [TestMethod]
    public async Task TestGetListByIdGreatherThan1AndAscAsyncReturnFirstOrderIdIs2()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = (await repository.GetListAsync(o => o.Id > 1, "Id", false)).ToList();
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 2);
    }

    [TestMethod]
    public async Task TestGetListByGreatherThan1AndDescAsyncReturnFirstOrderIdIs3()
    {
        var repository = await InitDataAsync();
        await repository.AddAsync(new Orders(3)
        {
            OrderNumber = 99998,
            Description = "Microsoft"
        });
        await repository.UnitOfWork.SaveChangesAsync();

        var paginatedList = (await repository.GetListAsync(o => o.Id > 1, "Id")).ToList();
        Assert.IsTrue(paginatedList.Count == 2 && paginatedList.First().Id == 3);
    }

    [TestMethod]
    public void TestPaginatedOptionsConstructorAndSortingIsNullReturnSortingIsNull()
    {
        int page = 1;
        int pageSize = 20;
        var pageOptions = new PaginatedOptions(page, pageSize);
        Assert.IsTrue(pageOptions.Sorting == null);
    }

    [TestMethod]
    public void TestPaginatedOptionsConstructorReturnSortingCountIs1()
    {
        int page = 1;
        int pageSize = 20;
        var pageOptions = new PaginatedOptions(page, pageSize, "Id");
        Assert.IsTrue(pageOptions.Page == page && pageOptions.PageSize == pageSize && pageOptions.Sorting!.Count == 1);
    }

    [TestMethod]
    public async Task TestRemoveIdEqual1ReturnCountIs1()
    {
        var repository = await InitDataAndReturnRepositoryAsync();
        await repository.RemoveAsync(1);
        await repository.UnitOfWork.SaveChangesAsync();
        Assert.IsTrue(await repository.GetCountAsync() == 1);
    }

    [TestMethod]
    public async Task TestRemoveIdEqual1Or2ReturnCountIs0()
    {
        var repository = await InitDataAndReturnRepositoryAsync();
        await repository.RemoveRangeAsync(new[] { 1, 2 });
        await repository.UnitOfWork.SaveChangesAsync();
        Assert.IsTrue(await repository.GetCountAsync() == 0);
    }
}
