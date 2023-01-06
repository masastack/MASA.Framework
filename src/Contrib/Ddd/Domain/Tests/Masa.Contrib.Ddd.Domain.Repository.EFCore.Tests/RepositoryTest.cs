// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore.Tests;

[TestClass]
public class RepositoryTest
{
    [TestInitialize]
    public void Initialize()
    {
    }

    [TestMethod]
    public async Task TestAddRangeReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.AddRangeAsync(It.IsAny<Orders[]>(), default))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.AddRangeAsync(orders);
        dbContext.Verify(context => context.AddRangeAsync(It.IsAny<IEnumerable<Orders>>(), default), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Never);
    }

    [TestMethod]
    public async Task TestAddRangeAndUseTransactionReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.AddRangeAsync(It.IsAny<Orders[]>(), default))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(uoW => uoW.UseTransaction).Returns(true).Verifiable();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);
        Assert.IsTrue(repository.UnitOfWork.UseTransaction);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.AddRangeAsync(orders);
        dbContext.Verify(context => context.AddRangeAsync(It.IsAny<IEnumerable<Orders>>(), default), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Once);
    }

    [TestMethod]
    public async Task TestUpdateRangeReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().UpdateRange(It.IsAny<IEnumerable<Orders>>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.UpdateRangeAsync(orders);
        dbContext.Verify(context => context.Set<Orders>().UpdateRange(It.IsAny<IEnumerable<Orders>>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Never);
    }

    [TestMethod]
    public async Task TestUpdateRangeAndUseTransactionReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().UpdateRange(It.IsAny<IEnumerable<Orders>>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(uoW => uoW.UseTransaction).Returns(true).Verifiable();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);
        Assert.IsTrue(repository.UnitOfWork.UseTransaction);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.UpdateRangeAsync(orders);
        dbContext.Verify(context => context.Set<Orders>().UpdateRange(It.IsAny<IEnumerable<Orders>>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Once);
    }

    [TestMethod]
    public async Task TestUpdateReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().Update(It.IsAny<Orders>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);

        var order = new Orders(999);
        await repository.UpdateAsync(order);
        dbContext.Verify(context => context.Set<Orders>().Update(It.IsAny<Orders>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Never);
    }

    [TestMethod]
    public async Task TestUpdateAndUseTransactionReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().Update(It.IsAny<Orders>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(uoW => uoW.UseTransaction).Returns(true).Verifiable();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);
        Assert.IsTrue(repository.UnitOfWork.UseTransaction);

        var order = new Orders(999);
        await repository.UpdateAsync(order);
        dbContext.Verify(context => context.Set<Orders>().Update(It.IsAny<Orders>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Once);
    }

    [TestMethod]
    public async Task TestRemoveRangeReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().RemoveRange(It.IsAny<IEnumerable<Orders>>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.RemoveRangeAsync(orders);
        dbContext.Verify(context => context.Set<Orders>().RemoveRange(It.IsAny<IEnumerable<Orders>>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Never);
    }

    [TestMethod]
    public async Task TestRemoveRangeAndUseTransactionReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().RemoveRange(It.IsAny<IEnumerable<Orders>>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(uoW => uoW.UseTransaction).Returns(true).Verifiable();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);
        Assert.IsTrue(repository.UnitOfWork.UseTransaction);

        var orders = new List<Orders>()
        {
            new(9999)
        };
        await repository.RemoveRangeAsync(orders);
        dbContext.Verify(context => context.Set<Orders>().RemoveRange(It.IsAny<IEnumerable<Orders>>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Once);
    }

    [TestMethod]
    public async Task TestRemoveReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().Remove(It.IsAny<Orders>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);

        var order = new Orders(999);
        await repository.RemoveAsync(order);
        dbContext.Verify(context => context.Set<Orders>().Remove(It.IsAny<Orders>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Never);
    }

    [TestMethod]
    public async Task TestRemoveAndUseTransactionReturnEntityStateEqualChangedAsync()
    {
        Mock<CustomDbContext> dbContext = new();
        dbContext
            .Setup(context => context.Set<Orders>().Remove(It.IsAny<Orders>()))
            .Verifiable();

        Mock<IUnitOfWork> unitOfWork = new();
        unitOfWork.Setup(uoW => uoW.UseTransaction).Returns(true).Verifiable();
        var repository = new Repository<CustomDbContext, Orders>(dbContext.Object, unitOfWork.Object);
        Assert.IsTrue(repository.EntityState == EntityState.UnChanged);
        Assert.IsTrue(repository.UnitOfWork.UseTransaction);

        var order = new Orders(999);
        await repository.RemoveAsync(order);
        dbContext.Verify(context => context.Set<Orders>().Remove(It.IsAny<Orders>()), Times.Once);
        unitOfWork.VerifySet(uoW => uoW.EntityState = EntityState.Changed, Times.Once);
        unitOfWork.VerifySet(uoW => uoW.CommitState = CommitState.UnCommited, Times.Once);
    }

    [TestMethod]
    public void TestUseRepository()
    {
        var services = new ServiceCollection();
        services.AddDbContext<CustomDbContext>();
        Mock<IUnitOfWork> unitOfWork = new();
        services.AddScoped(_ => unitOfWork.Object);
        Mock<IDomainEventOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(options => options.Assemblies).Returns(() => new[] { typeof(Orders).Assembly });
        dispatcherOptions.Setup(options => options.Services).Returns(() => services);
        dispatcherOptions.Object.UseRepository<CustomDbContext>();
        var serviceProvider = services.BuildServiceProvider();
        var orderRepository = serviceProvider.GetService<IRepository<Orders>>();
        var orderItemRepository = serviceProvider.GetService<IRepository<OrderItem>>();
        Assert.IsNotNull(orderRepository);
        Assert.IsNotNull(orderItemRepository);
    }

    [TestMethod]
    public void TestUseRepositoryBySpecifyEntityType()
    {
        var services = new ServiceCollection();
        services.AddDbContext<CustomDbContext>();
        Mock<IUnitOfWork> unitOfWork = new();
        services.AddScoped(_ => unitOfWork.Object);
        Mock<IDomainEventOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(options => options.Assemblies).Returns(() => new[] { typeof(Orders).Assembly });
        dispatcherOptions.Setup(options => options.Services).Returns(() => services);
        dispatcherOptions.Object.UseRepository<CustomDbContext>(typeof(Orders));
        var serviceProvider = services.BuildServiceProvider();
        var orderRepository = serviceProvider.GetService<IRepository<Orders>>();
        var orderItemRepository = serviceProvider.GetService<IRepository<OrderItem>>();
        Assert.IsNotNull(orderRepository);
        Assert.IsNull(orderItemRepository);
    }
}
