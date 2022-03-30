namespace Masa.Contrib.Isolation.UoW.EF.Tests;

[TestClass]
public class TestIsolation : TestBase
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void TestUseIsolationUoW()
    {
        Mock<IEventBusBuilder> eventBuilder = new();
        eventBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            eventBuilder.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString), _ =>
            {
            });
        }, "Tenant isolation and environment isolation use at least one");
    }

    [TestMethod]
    public void TestUseIsolationUoW2()
    {
        Mock<IEventBusBuilder> eventBuilder = new();
        eventBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            eventBuilder.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString), null!);
        });
    }

    [TestMethod]
    public void TestUseIsolationUoW3()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString), _ =>
            {
            });
        }, "Tenant isolation and environment isolation use at least one");
    }

    [TestMethod]
    public void TestUseIsolationUoW4()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(), null!);
        });
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseEnvironment()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString),
            isolationBuilder => isolationBuilder.UseEnvironment());

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IEnvironmentContext>());
        Assert.IsNotNull(serviceProvider.GetService<IEnvironmentSetter>());
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseMultiEnvironment()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString),
            isolationBuilder => isolationBuilder.UseEnvironment().UseEnvironment());

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IEnvironmentContext>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IEnvironmentSetter>().Count() == 1);
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseTenant()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString),
            isolationBuilder => isolationBuilder.UseMultiTenancy());

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ITenantContext>());
        Assert.IsNotNull(serviceProvider.GetService<ITenantSetter>());
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseMultiTenant()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString),
            isolationBuilder => isolationBuilder.UseMultiTenancy().UseMultiTenancy());

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<ITenantContext>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<ITenantSetter>().Count() == 1);
    }

    [TestMethod]
    public void TestUseIsolation()
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        _services.AddSingleton<IConfiguration>(configurationRoot);
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(),
            isolationBuilder => isolationBuilder.UseMultiTenancy().UseEnvironment());
        var serviceProvider = _services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "test1");

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var unifOfWorkNew = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew = unifOfWorkNew.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();

        Assert.IsNull(unitOfWorkAccessorNew.CurrentDbContextOptions);

        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<ITenantContext>().CurrentTenant == null);

        Assert.IsTrue(string.IsNullOrEmpty(unifOfWorkNew.ServiceProvider.GetRequiredService<IEnvironmentContext>().CurrentEnvironment));

        unifOfWorkNew.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<ITenantContext>().CurrentTenant!.Id ==
            "00000000-0000-0000-0000-000000000002");
        unifOfWorkNew.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("dev");

        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<IEnvironmentContext>().CurrentEnvironment == "dev");

        var dbContext = unifOfWorkNew.ServiceProvider.GetRequiredService<CustomDbContext>();

        Assert.IsTrue(GetDataBaseConnectionString(dbContext) == "test1" &&
            unitOfWorkAccessorNew.CurrentDbContextOptions!.ConnectionString == "test1");

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("development");
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "test1" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "test1");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("development");
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "test2" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "test2");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("production");
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "test3" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "test3");
    }

    [TestMethod]
    public void TestUseEnvironment()
    {
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = "test4";
            option.Isolations = new List<DbConnectionOptions>()
            {
                new()
                {
                    ConnectionString = "test5",
                    Environment = "dev"
                },
                new()
                {
                    ConnectionString = "test6",
                    Environment = "pro"
                }
            };
        });
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(),
            isolationBuilder => isolationBuilder.UseEnvironment());
        var serviceProvider = _services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "test4");

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("dev");
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "test5" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "test5");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("pro");
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "test6" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "test6");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("staging");
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "test4" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "test4");
    }

    [TestMethod]
    public void TestUseMultiTenancy()
    {
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = "test7";
            option.Isolations = new List<DbConnectionOptions>()
            {
                new()
                {
                    ConnectionString = "test8",
                    TenantId = "1"
                },
                new()
                {
                    ConnectionString = "test9",
                    TenantId = "2"
                }
            };
        });
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(),
            isolationBuilder => isolationBuilder.UseMultiTenancy<int>());
        var serviceProvider = _services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "test7");

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("1"));
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "test8" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "test8");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("2"));
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "test9" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "test9");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(null!);
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "test7" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "test7");
    }

    [TestMethod]
    public void TestIsolationBuilder()
    {
        var services = new ServiceCollection();
        var isolationBuilder = new IsolationBuilder(services);
        Assert.IsTrue(isolationBuilder.EnvironmentKey == "ASPNETCORE_ENVIRONMENT");
        Assert.IsTrue(isolationBuilder.TenantKey == "__tenant");
        Assert.IsTrue(isolationBuilder.TenantParsers.Count == 6);
        Assert.IsTrue(isolationBuilder.EnvironmentParsers.Count == 1);

        Assert.IsTrue(isolationBuilder.SetTenantKey("tenantId").TenantKey == "tenantId");
        Assert.IsTrue(isolationBuilder.SetEnvironmentKey("dev").EnvironmentKey == "dev");
        Assert.IsTrue(isolationBuilder.SetEnvironmentParsers(new List<IEnvironmentParserProvider>()).EnvironmentParsers.Count == 0);
        Assert.IsTrue(isolationBuilder.SetTenantParsers(new List<ITenantParserProvider>()).EnvironmentParsers.Count == 0);
    }

    private string GetDataBaseConnectionString(CustomDbContext dbContext) => dbContext.Database.GetConnectionString()!;
}
