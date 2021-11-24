namespace MASA.Contrib.Data.UoW.EF.Tests;

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
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<CustomerDbContext>());
    }

    [TestMethod]
    public void TestAddMultUoW()
    {
        _options.Object
            .UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection))
            .UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));

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
    public async Task TestSaveChangesAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        Mock<CustomerDbContext> customerDbContext = new();
        customerDbContext.Setup(dbContext => dbContext.SaveChangesAsync(default)).Verifiable();
        var uoW = new UnitOfWork<CustomerDbContext>(customerDbContext.Object, null);
        await uoW.SaveChangesAsync(default);
        customerDbContext.Verify(dbContext => dbContext.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task TestUseTranscationAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();

        var transaction = uoW.Transaction;
        Users user = new Users()
        {
            Name = Guid.NewGuid().ToString()
        };
        dbContext.Add(user);
        await uoW.SaveChangesAsync();
        await uoW.RollbackAsync();

        Assert.IsTrue(dbContext.User.ToList().Count() == 0);
    }

    [TestMethod]
    public async Task TestNotUseTranscationAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = new UnitOfWork<CustomerDbContext>(dbContext, null);

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
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = new UnitOfWork<CustomerDbContext>(dbContext, null);
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await uoW.CommitAsync());
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = new UnitOfWork<CustomerDbContext>(dbContext, null);
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
    public async Task TestCloseRollbackAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection), true);
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await Assert.ThrowsExceptionAsync<DbUpdateException>(async () => await uoW.CommitAsync());
    }

    [TestMethod]
    public async Task TestAddLoggerAndCloseRollbackAsync()
    {
        _options.Object.Services.AddLogging();
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection), true);
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await Assert.ThrowsExceptionAsync<DbUpdateException>(async () => await uoW.CommitAsync());
    }

    [TestMethod]
    public async Task TestOpenRollbackAsync()
    {
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
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
        _options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.Database.EnsureCreated();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await uoW.CommitAsync();

        Assert.IsTrue(!await dbContext.User.AnyAsync());
    }
}
