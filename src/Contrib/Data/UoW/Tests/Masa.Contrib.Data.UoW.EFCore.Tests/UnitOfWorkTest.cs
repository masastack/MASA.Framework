// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore.Tests;

[TestClass]
public class UnitOfWorkTest : TestBase
{
    private UnitOfWork<CustomDbContext> _unitOfWork;
    private CustomDbContext _dbContext;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(masaDbContextBuilder => masaDbContextBuilder.UseTestSqlite(Connection));
        var serviceProvider = services.BuildServiceProvider();
        _dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        _dbContext.Database.EnsureCreated();
        _unitOfWork = new UnitOfWork<CustomDbContext>(serviceProvider);
    }

    [TestMethod]
    public void TestUnitOfWork()
    {
        _unitOfWork.UseTransaction = false;
        Assert.ThrowsException<NotSupportedException>(() => _unitOfWork.Transaction);
        Assert.AreEqual(false, _unitOfWork.TransactionHasBegun);
        Assert.AreEqual(false, _unitOfWork.DisableRollbackOnFailure);
        Assert.AreEqual(EntityState.UnChanged, _unitOfWork.EntityState);
        Assert.AreEqual(CommitState.Commited, _unitOfWork.CommitState);
    }

    [TestMethod]
    public async Task TestCommitByUseTransactionIsNullAsync()
    {
        Assert.AreEqual(null, _unitOfWork.UseTransaction);
        var transaction = _unitOfWork.Transaction;
        Assert.IsNotNull(transaction);
        var user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        _unitOfWork.EntityState = EntityState.Changed;
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public async Task TestCommitByUseTransactionIsFalseAsync()
    {
        _unitOfWork.UseTransaction = false;
        var user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        _unitOfWork.EntityState = EntityState.Changed;
        await _unitOfWork.SaveChangesAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public async Task TestCommitByUseTransactionIsFalseAndEntityStateIsUnChangedAsync()
    {
        _unitOfWork.UseTransaction = false;
        var user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        await _unitOfWork.SaveChangesAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public async Task TestCommitByUseTransactionIsTrueAsync()
    {
        _unitOfWork.UseTransaction = true;
        _ = _unitOfWork.Transaction;
        var user = new Users
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        _unitOfWork.EntityState = EntityState.Changed;
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public async Task TestRollbackAsync()
    {
        _unitOfWork.UseTransaction = true;
        _ = _unitOfWork.Transaction;
        var user = new Users
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        _unitOfWork.EntityState = EntityState.Changed;
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.RollbackAsync();
        await _unitOfWork.CommitAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public async Task TestRollbackByUseTransactionIsFalseAsync()
    {
        _unitOfWork.UseTransaction = false;
        Assert.AreEqual(false, _unitOfWork.TransactionHasBegun);
        var user = new Users
        {
            Name = Guid.NewGuid().ToString()
        };
        _dbContext.Add(user);
        _unitOfWork.EntityState = EntityState.Changed;
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.RollbackAsync();
        await _unitOfWork.CommitAsync();

        var count = await _dbContext.User.CountAsync();
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public void TestAddUoWAndNullServices()
    {
        var options = new Mock<IDispatcherOptions>();
        Assert.ThrowsException<MasaArgumentException>(() => options.Object.UseUoW<CustomDbContext>());
    }

    [TestMethod]
    public void TestAddUoWAndUseSqlLite()
    {
        Mock<IDispatcherOptions> options = new();
        options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        options.Object.UseUoW<CustomDbContext>(masaDbContextBuilder => masaDbContextBuilder.UseTestSqlite(_connectionString));
        var serviceProvider = options.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<CustomDbContext>());
        Assert.IsNotNull(serviceProvider.GetService<IDbConnectionStringProvider>());
        Assert.IsNotNull(serviceProvider.GetService<IUnitOfWork>());
        Assert.IsNotNull(serviceProvider.GetService<IUnitOfWorkAccessor>());
    }

    [TestMethod]
    public void TestAddMultUoW()
    {
        Mock<IDispatcherOptions> options = new();
        options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        options.Object
            .UseUoW<CustomDbContext>(masaDbContextBuilder => masaDbContextBuilder.UseTestSqlite(_connectionString))
            .UseUoW<CustomDbContext>(masaDbContextBuilder => masaDbContextBuilder.UseTestSqlite(_connectionString));

        var serviceProvider = options.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IUnitOfWork>().Count() == 1);
        Assert.AreEqual(1, serviceProvider.GetServices<IDbConnectionStringProvider>().Count());
        Assert.AreEqual(1, serviceProvider.GetServices<IUnitOfWork>().Count());
        Assert.AreEqual(1, serviceProvider.GetServices<IUnitOfWorkAccessor>().Count());
    }

    [TestMethod]
    public void TestUnitOfWorkByEventBusBuilder()
    {
        var services = new ServiceCollection();
        services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = _connectionString
            };
        });
        Mock<IEventBusBuilder> eventBuilder = new();
        eventBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
        eventBuilder.Object.UseUoW<CustomDbContext>(options => options.UseSqlite());

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IUnitOfWorkManager>());
        Assert.IsNotNull(serviceProvider.GetService<IUnitOfWorkAccessor>());
        Assert.IsNotNull(serviceProvider.GetService<IUnitOfWork>());
    }
}
