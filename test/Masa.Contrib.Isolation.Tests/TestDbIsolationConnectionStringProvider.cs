namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class TestDbIsolationConnectionStringProvider
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsync()
    {
        string defaultConnectionString = "data source=test1;";
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(defaultConnectionString);
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, null!);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == defaultConnectionString);
    }

    [TestMethod]
    public async Task TestGetConnectionString2Async()
    {
        string defaultConnectionString = "data source=test1;";

        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;

        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == defaultConnectionString);
    }

    [TestMethod]
    public async Task TestGetConnectionString3Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    Environment = "pro",
                    ConnectionString = "data source=test3;"
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    }

    [TestMethod]
    public async Task TestGetConnectionString4Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    Environment = "pro",
                    ConnectionString = "data source=test3;"
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("Staging").Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    }

    [TestMethod]
    public async Task TestGetConnectionString5Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    ConnectionString = "data source=test3;"
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test2;");
    }

    [TestMethod]
    public async Task TestGetConnectionString6Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    ConnectionString = "data source=test3;"
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    }

    [TestMethod]
    public async Task TestGetConnectionString7Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    ConnectionString = "data source=test3;"
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("11")).Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    }

    [TestMethod]
    public async Task TestGetConnectionString8Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("Staging").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("11")).Verifiable();
        var provider =
            new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    }

    [TestMethod]
    public async Task TestGetConnectionString9Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("dev").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
        var provider =
            new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test2;");
    }

    [TestMethod]
    public async Task TestGetConnectionString10Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("dev").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
        var provider =
            new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    }

    [TestMethod]
    public async Task TestGetConnectionString11Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test4;");
    }

    [TestMethod]
    public async Task TestGetConnectionString12Async()
    {
        _services.AddLogging();
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
        var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test4;");
    }

    [TestMethod]
    public async Task TestGetConnectionString13Async()
    {
        string defaultConnectionString = "data source=test1;";
        _services.Configure<IsolationDbConnectionOptions>(option => option.DefaultConnection = defaultConnectionString);
        var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = defaultConnectionString;
            option.Isolations = new List<DbConnectionOptions>
            {
                new()
                {
                    TenantId = "1",
                    Environment = "dev",
                    ConnectionString = "data source=test2;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "dev",
                    ConnectionString = "data source=test3;"
                },
                new()
                {
                    TenantId = "2",
                    Environment = "pro",
                    ConnectionString = "data source=test4;"
                },
                new()
                {
                    TenantId = "*",
                    Environment = "pro",
                    ConnectionString = "data source=test5;",
                    Score = 99
                }
            };
        });
        var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();

        Mock<IEnvironmentContext> environmentContext = new();
        environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();

        Mock<ITenantContext> tenantContext = new();
        tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("10")).Verifiable();
        var provider =
            new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
        Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test5;");
    }

}
