using Masa.Contrib.Isolation.MultiTenant;
using Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenant;

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
            isolationBuilder => isolationBuilder.UseMultiTenant());

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
            isolationBuilder => isolationBuilder.UseMultiTenant().UseMultiTenant());

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
            isolationBuilder => isolationBuilder.UseMultiTenant().UseEnvironment());
        var serviceProvider = _services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test1");

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

        Assert.IsTrue(GetDataBaseConnectionString(dbContext) == "data source=test1" &&
            unitOfWorkAccessorNew.CurrentDbContextOptions!.ConnectionString == "data source=test1");

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("development");
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test1" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test1");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("development");
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test2" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test2");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("production");
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test3" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test3");
    }

    [TestMethod]
    public void TestUseEnvironment()
    {
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = "data source=test4";
            option.Isolations = new List<DbConnectionOptions>()
            {
                new()
                {
                    ConnectionString = "data source=test5",
                    Environment = "dev"
                },
                new()
                {
                    ConnectionString = "data source=test6",
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
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test4");

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("dev");
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test5" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test5");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("pro");
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test6" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test6");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IEnvironmentSetter>().SetEnvironment("staging");
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test4" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test4");
    }

    [TestMethod]
    public void TestUseMultiTenant()
    {
        _services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = "data source=test7";
            option.Isolations = new List<DbConnectionOptions>()
            {
                new()
                {
                    ConnectionString = "data source=test8",
                    TenantId = "1"
                },
                new()
                {
                    ConnectionString = "data source=test9",
                    TenantId = "2"
                }
            };
        });
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(dbOptionBuilder => dbOptionBuilder.UseSqlite(),
            isolationBuilder => isolationBuilder.UseMultiTenant<int>());
        var serviceProvider = _services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test7");

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("1"));
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test8" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test8");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(new Tenant("2"));
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test9" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test9");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<ITenantSetter>().SetTenant(null!);
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test7" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test7");
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

    [TestMethod]
    public async Task TestCookieTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Cookies = new RequestCookieCollection
                {
                    {
                        tenantKey, "1"
                    }
                }
            }
        };
        var provider = new CookieTenantParserProvider();
        Assert.IsTrue(provider.Name == "Cookie");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestCookieTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Cookies = new RequestCookieCollection()
            }
        };
        var provider = new CookieTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestCookieTenantParser3Async()
    {
        var services = new ServiceCollection();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var provider = new CookieTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestFormTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Form = new FormCollection(new Dictionary<string, StringValues>()
                    {
                        { tenantKey, "1" }
                    }
                )
            }
        };
        var provider = new FormTenantParserProvider();
        Assert.IsTrue(provider.Name == "Form");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestFormTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Form = new FormCollection(new Dictionary<string, StringValues>())
            }
        };
        var provider = new FormTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestFormTenantParser3Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                QueryString = QueryString.Create(tenantKey, "1")
            }
        };
        var provider = new FormTenantParserProvider();
        Assert.IsTrue(provider.Name == "Form");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestHeaderTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers =
                {
                    { tenantKey, "1" }
                }
            }
        };
        var provider = new HeaderTenantParserProvider();
        Assert.IsTrue(provider.Name == "Header");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestHeaderTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers = { }
            }
        };
        var provider = new HeaderTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestHttpContextItemTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Items = new Dictionary<object, object?>()
            {
                { tenantKey, "1" }
            }
        };
        var provider = new HttpContextItemTenantParserProvider();
        Assert.IsTrue(provider.Name == "Items");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestHttpContextItemTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Items = new Dictionary<object, object?>()
        };
        var provider = new HttpContextItemTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestHttpContextItemTenantParser3Async()
    {
        var services = new ServiceCollection();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var provider = new HttpContextItemTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestQueryStringTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request = { QueryString = QueryString.Create(tenantKey, "1") }
        };
        var provider = new QueryStringTenantParserProvider();
        Assert.IsTrue(provider.Name == "QueryString");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestQueryStringTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request = { QueryString = new QueryString() }
        };
        var provider = new QueryStringTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestQueryStringTenantParser3Async()
    {
        var services = new ServiceCollection();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var provider = new QueryStringTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestRouteTenantParserAsync()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                RouteValues = new RouteValueDictionary()
                {
                    { tenantKey, "1" }
                }
            }
        };
        var provider = new RouteTenantParserProvider();
        Assert.IsTrue(provider.Name == "Route");
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsTrue(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    public async Task TestRouteTenantParser2Async()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            Request =
            {
                RouteValues = new RouteValueDictionary()
            }
        };
        var provider = new RouteTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestRouteTenantParser3Async()
    {
        var services = new ServiceCollection();
        string tenantKey = "tenant";
        Mock<ITenantSetter> tenantSetter = new();
        tenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>())).Verifiable();
        services.AddScoped(_ => tenantSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.TenantKey = tenantKey;
        });
        var provider = new RouteTenantParserProvider();
        var handler = await provider.ResolveAsync(services.BuildServiceProvider());
        Assert.IsFalse(handler);
        tenantSetter.Verify(setter => setter.SetTenant(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task TestEnvironmentVariablesParserAsync()
    {
        var services = new ServiceCollection();
        Mock<IEnvironmentSetter> environmentSetter = new();
        string environmentKey = "env";
        environmentSetter.Setup(setter => setter.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.EnvironmentKey = environmentKey;
        });
        System.Environment.SetEnvironmentVariable(environmentKey, "dev");
        var serviceProvider = services.BuildServiceProvider();
        var environmentVariablesParserProvider = new EnvironmentVariablesParserProvider();
        var handler = await environmentVariablesParserProvider.ResolveAsync(serviceProvider);
        Assert.IsTrue(handler);
    }

    [TestMethod]
    public async Task TestEnvironmentVariablesParser2Async()
    {
        var services = new ServiceCollection();
        Mock<IEnvironmentSetter> environmentSetter = new();
        string environmentKey = "env";
        System.Environment.SetEnvironmentVariable(environmentKey, "");
        environmentSetter.Setup(setter => setter.SetEnvironment(It.IsAny<string>())).Verifiable();
        services.AddScoped(_ => environmentSetter.Object);
        services.Configure<IsolationOptions>(option =>
        {
            option.EnvironmentKey = environmentKey;
        });
        var serviceProvider = services.BuildServiceProvider();
        var environmentVariablesParserProvider = new EnvironmentVariablesParserProvider();
        var handler = await environmentVariablesParserProvider.ResolveAsync(serviceProvider);
        Assert.IsFalse(handler);
    }

    [TestMethod]
    public void TestGetDbContextOptionsList()
    {
        var services = new ServiceCollection();
        services.Configure<IsolationDbConnectionOptions>(option =>
        {
            option.DefaultConnection = "data source=test2";
            option.Isolations = new()
            {
                new()
                {
                    Environment = "dev",
                    ConnectionString = "data source=test3"
                },
                new()
                {
                    Environment = "pro",
                    ConnectionString = "data source=test4"
                }
            };
        });
        services.AddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetRequiredService<IDbConnectionStringProvider>();
        Assert.IsTrue(provider.DbContextOptionsList.Distinct().Count() == 3);
    }

    private string GetDataBaseConnectionString(CustomDbContext dbContext) => dbContext.Database.GetConnectionString()!;

}
