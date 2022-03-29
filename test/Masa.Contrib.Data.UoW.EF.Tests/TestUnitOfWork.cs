using Microsoft.Extensions.Configuration;

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
        Assert.ThrowsException<ArgumentNullException>(() => options.Object.UseUoW<CustomDbContext>());
    }

    [TestMethod]
    public void TestAddUoWAndUseSqlLite()
    {
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(_connectionString));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<CustomDbContext>());
    }

    [TestMethod]
    public void TestAddMultUoW()
    {
        _options.Object
            .UseUoW<CustomDbContext>(options => options.UseSqlite(_connectionString))
            .UseUoW<CustomDbContext>(options => options.UseSqlite(_connectionString));

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
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
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
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomDbContext>(serviceProvider);

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
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(_connectionString));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomDbContext>(serviceProvider);
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await uoW.CommitAsync());
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = new UnitOfWork<CustomDbContext>(serviceProvider);
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
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
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
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var uoW = serviceProvider.GetRequiredService<IUnitOfWork>();
        var user = new Users();
        var transcation = uoW.Transaction;
        dbContext.User.Add(user);
        await uoW.CommitAsync();

        Assert.IsTrue(!await dbContext.User.AnyAsync());
    }

    [TestMethod]
    public void TestDataConnectionString()
    {
        IConfiguration configuration = new ConfigurationManager();
        _options.Object.Services.AddSingleton(_ => configuration);
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var dataConnectionStringProvider = serviceProvider.GetRequiredService<IDataConnectionStringProvider>();
        Assert.IsTrue(dataConnectionStringProvider.DbContextOptionsList.Count == 1 && dataConnectionStringProvider.DbContextOptionsList.Any(option => option.ConnectionString == null && option.Connection == null));
    }

    [TestMethod]
    public async Task TestUnitOfWorkManagerAsync()
    {
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite(Connection));
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var dbContext2 = serviceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(dbContext.Equals(dbContext2));

        var newUnitOfWork = await unitOfWorkManager.CreateDbContextAsync(new Masa.BuildingBlocks.Data.UoW.Options.MasaDbContextOptions(Connection));
        Assert.IsFalse(newUnitOfWork.Equals(unitOfWork));
        var newDbContext = newUnitOfWork.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsFalse(dbContext.Equals(newDbContext));

        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await unitOfWorkManager.CreateDbContextAsync(new BuildingBlocks.Data.UoW.Options.MasaDbContextOptions("")));
    }
}
