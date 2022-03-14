using Masa.Contrib.Data.Contracts.EF.Tests.Infrastructure;

namespace Masa.Contrib.Data.Contracts.EF.Tests;

[TestClass]
public class SoftDeleteTest : IDisposable
{
    protected readonly SqliteConnection _connection;

    public SoftDeleteTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
    }

    [TestMethod]
    public void UseNotUseUoW()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(option =>
        {
            option.UseSqlite(_connection);
            Assert.ThrowsException<Exception>(option.UseSoftDelete, "Please add UoW first.");
        });
    }

    [TestMethod]
    public void TestUseSoftDelete()
    {
        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(u => u.Transaction).Verifiable();
        var services = new ServiceCollection();
        services.AddScoped(serviceProvider => uoW.Object);
        services.AddMasaDbContext<CustomDbContext>(option =>
        {
            option.UseSoftDelete().UseSqlite(_connection);
        });

        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();

        dbContext.Set<Students>().Add(new Students()
        {
            Name = "Tom",
            Age = 18
        });
        dbContext.SaveChanges();
        Assert.IsTrue(dbContext.Students.Count() == 1);
        uoW.Verify(u => u.Transaction, Times.Never);

        var student = dbContext.Students.FirstOrDefault(s => s.Name == "Tom");
        Assert.IsNotNull(student);
        dbContext.Set<Students>().Remove(student);
        dbContext.SaveChanges();

        Assert.IsTrue(!dbContext.Students.Any());
    }

    [TestMethod]
    public void TestUseMultiSoftDelete()
    {
        Mock<IUnitOfWork> uoW = new();
        uoW.Setup(u => u.Transaction).Verifiable();
        var services = new ServiceCollection();
        services.AddScoped(serviceProvider => uoW.Object);
        services.AddMasaDbContext<CustomDbContext>(option =>
        {
            option.UseSqlite(_connection);
            option.UseSoftDelete().UseSoftDelete();
        });
    }
}
