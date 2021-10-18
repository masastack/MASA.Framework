namespace MASA.Contrib.Data.Uow.EF.Tests;

[TestClass]
public class TestUnitOfWork : TestBase
{
    [TestMethod]
    public void TestAddUowAndNullServices()
    {
        var options = new Mock<IDispatcherOptions>();
        Assert.ThrowsException<ArgumentNullException>(() => options.Object.UseUoW<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddUow()
    {
        var options = new Mock<IDispatcherOptions>();
        options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        options.Object.UseUoW<CustomerDbContext>();
        var serviceProvider = options.Object.Services.BuildServiceProvider();
        Assert.ThrowsException<InvalidOperationException>(() => serviceProvider.GetRequiredService<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddUowAndUseSqlLite()
    {
        var options = new Mock<IDispatcherOptions>();
        options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = options.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddMultUow()
    {
        var options = new Mock<IDispatcherOptions>();
        options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
        options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection)).UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = options.Object.Services.BuildServiceProvider();

        Assert.IsTrue(serviceProvider.GetServices<IUnitOfWork>().Count() == 1);
    }

    [TestMethod]
    public async Task TestNoTransactionAndCommitAsync()
    {
        var serviceProviderAndDbContext = base.CreateDefault();
        var serviceProvider = serviceProviderAndDbContext.serviceProvider;
        var dbContext = serviceProviderAndDbContext.dbContext;

        await using (var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>())
        {
            var transcation = unitOfWork.Transaction;
            Assert.IsTrue(unitOfWork == serviceProvider.GetRequiredService<ITransaction>().UnitOfWork);

            Users user = new Users()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            dbContext.Add(user);
            await unitOfWork.CommitAsync();

            Assert.IsTrue(dbContext.User.Any(user => user.Id == user.Id));
        }
    }

    [TestMethod]
    public async Task TestUseTransactionAndCommitAsync()
    {
        var serviceProviderAndDbContext = base.CreateDefault();
        var serviceProvider = serviceProviderAndDbContext.serviceProvider;
        var dbContext = serviceProviderAndDbContext.dbContext;

        using (var transcation = await dbContext.Database.BeginTransactionAsync())
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            Users user = new Users()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            dbContext.Add(user);
            await unitOfWork.CommitAsync(); ;
        }
    }

    [TestMethod]
    public async Task TestNoTransactionAsync()
    {
        var serviceProviderAndDbContext = base.CreateDefault();
        var serviceProvider = serviceProviderAndDbContext.serviceProvider;
        var dbContext = serviceProviderAndDbContext.dbContext;

        await using (var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>())
        {
            Users user = new Users()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString().Substring(0, 6)
            };
            dbContext.Add(user);

            await unitOfWork.SaveChangesAsync();

            await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
            {
                await unitOfWork.RollbackAsync();
            });

            Assert.IsTrue(dbContext.User.Any(user => user.Id == user.Id));
        }
    }
}
