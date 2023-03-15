// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class DbIsolationConnectionStringProviderTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
    }

    // [TestMethod]
    // public async Task TestGetConnectionStringAsync()
    // {
    //     string defaultConnectionString = "data source=test1;";
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     unitOfWorkAccessor.CurrentDbContextOptions = new MasaDbContextConfigurationOptions(defaultConnectionString);
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, null!);
    //
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == defaultConnectionString);
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString2Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == connectionStrings.DefaultConnection);
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString3Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test3;"
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString4Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test3;"
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("Staging").Verifiable();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString5Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 ConnectionString = "data source=test3;"
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test2;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString6Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 ConnectionString = "data source=test3;"
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString7Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 ConnectionString = "data source=test3;"
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("11")).Verifiable();
    //     var provider = new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, null, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString8Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("Staging").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("11")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test1;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString9Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("dev").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("1")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test2;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString10Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("dev").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test3;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString11Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test4;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString12Async()
    // {
    //     _services.AddLogging();
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("2")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test4;");
    // }
    //
    // [TestMethod]
    // public async Task TestGetConnectionString13Async()
    // {
    //     var connectionStrings = new ConnectionStrings()
    //     {
    //         DefaultConnection = "data source=test1;"
    //     };
    //     _services.Configure<IsolationDbConnectionOptions>(option => option.ConnectionStrings = connectionStrings);
    //     var unitOfWorkAccessor = _services.BuildServiceProvider().GetService<IUnitOfWorkAccessor>()!;
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = connectionStrings;
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 TenantId = "1",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test2;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3;"
    //             },
    //             new()
    //             {
    //                 TenantId = "2",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4;"
    //             },
    //             new()
    //             {
    //                 TenantId = "*",
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test5;",
    //                 Score = 99
    //             }
    //         };
    //     });
    //     var options = _services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<IsolationDbConnectionOptions>>();
    //
    //     Mock<IMultiEnvironmentContext> environmentContext = new();
    //     environmentContext.Setup(context => context.CurrentEnvironment).Returns("pro").Verifiable();
    //
    //     Mock<IMultiTenantContext> tenantContext = new();
    //     tenantContext.Setup(context => context.CurrentTenant).Returns(new Tenant("10")).Verifiable();
    //     var provider =
    //         new DefaultDbIsolationConnectionStringProvider(unitOfWorkAccessor, options, environmentContext.Object, tenantContext.Object);
    //     Assert.IsTrue(await provider.GetConnectionStringAsync() == "data source=test5;");
    // }

}
