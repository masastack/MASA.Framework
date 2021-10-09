namespace MASA.Contrib.Data.Uow.EF.Tests;

[TestClass]
public class TestUnitOfWork : TestBase
{
    [TestMethod]
    public void TestAddUow()
    {
        var serviceProvider = base.CreateProviderByEmptyDbConnectionString();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        Assert.ThrowsException<InvalidOperationException>(() => dbContext.Database.EnsureCreated());
    }

    [TestMethod]
    public async Task TestCommitAsync()
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

            Assert.IsFalse(dbContext.User.Any(user => user.Id == user.Id));
        }
    }

    [TestMethod]
    public async Task TestNoTransactionAndCommitAsync()
    {
        var serviceProviderAndDbContext = base.CreateDefault();
        var serviceProvider = serviceProviderAndDbContext.serviceProvider;
        var dbContext = serviceProviderAndDbContext.dbContext;

        await using (var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>())
        {
            Assert.IsTrue(unitOfWork == serviceProvider.GetRequiredService<ITransaction>().UnitOfWork);

            Users user = new Users()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            dbContext.Add(user);
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => { await unitOfWork.CommitAsync(); });
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
