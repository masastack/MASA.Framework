namespace Masa.Contrib.Data.UoW.EF.Tests;

[TestClass]
public class TestUnitOfWork : TestBase
{
    private Mock<IDispatcherOptions> _options;

    [TestInitialize]
    public void Initialize()
    {
        _options = new();
        _options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
    }

    [TestMethod]
    public void TestAddUoWAndNullServices()
    {
        var options = new Mock<IDispatcherOptions>();
        Assert.ThrowsException<ArgumentNullException>(() => options.Object.UseUoW<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddUoW()
    {
        _options.Object.UseUoW<CustomerDbContext>();
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        Assert.ThrowsException<InvalidOperationException>(()
            => serviceProvider.GetRequiredService<CustomerDbContext>()
        );
    }

    [TestMethod]
    public void TestAddUoWAndUseSqlLite()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connectionString));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddMultUoW()
    {
        _options.Object
            .UseUoW<CustomerDbContext>(options => options.UseSqlite(_connectionString))
            .UseUoW<CustomerDbContext>(options => options.UseSqlite(_connectionString));

        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IUnitOfWork>().Count() == 1);
    }

    [TestMethod]
    public void TestTransaction()
    {
        Mock<IUnitOfWork> uoW = new();
        Assert.IsTrue(new Transaction(uoW.Object).UnitOfWork!.Equals(uoW.Object));
    }

    [TestMethod]
    public async Task TestUseTranscationAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();

        var transaction = uoW.Transaction;
        Users user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        dbContext.Add(user);
        await uoW.SaveChangesAsync();
        await uoW.RollbackAsync();

        Assert.IsTrue(!dbContext.User.ToList().Any());
    }

    [TestMethod]
    public async Task TestNotUseTranscationAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomerDbContext>(serviceProvider);

        Users user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        dbContext.Add(user);
        await uoW.SaveChangesAsync();
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await uoW.RollbackAsync());
    }

    [TestMethod]
    public async Task TestNotTransactionCommitAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connectionString));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomerDbContext>(serviceProvider);
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await uoW.CommitAsync());
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomerDbContext>(serviceProvider);
        var user = new Users()
        {
            Name = "Tom"
        };
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await uoW.SaveChangesAsync();
        await uoW.CommitAsync();

        Assert.IsTrue(dbContext.User.ToList().Count == 1);
    }

    [TestMethod]
    public async Task TestOpenRollbackAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await uoW.CommitAsync();

        Assert.IsTrue(!await dbContext.User.AnyAsync());
    }

    [TestMethod]
    public async Task TestAddLoggerAndOpenRollbackAsync()
    {
        _options.Object.Services.AddLogging();
        _options.Object.UseUoW<CustomerDbContext>(options => options.DbContextOptionsBuilder.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await uoW.CommitAsync();

        Assert.IsTrue(!await dbContext.User.AnyAsync());
    }
}
