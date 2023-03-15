// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.UoW.EFCore.Tests;

[TestClass]
public class IsolationTest : TestBase
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
            eventBuilder.Object.UseIsolationUoW<CustomDbContext>(_ =>
            {
            }, dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));
        }, "Tenant isolation and environment isolation use at least one");
    }

    [TestMethod]
    public void TestUseIsolationUoW2()
    {
        Mock<IEventBusBuilder> eventBuilder = new();
        eventBuilder.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            eventBuilder.Object.UseIsolationUoW<CustomDbContext>(null!,
                dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));
        });
    }

    [TestMethod]
    public void TestUseIsolationUoW3()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(_ =>
            {
            }, dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));
        }, "Tenant isolation and environment isolation use at least one");
    }

    [TestMethod]
    public void TestUseIsolationUoW4()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(null!, dbOptionBuilder => dbOptionBuilder.UseSqlite());
        });
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseEnvironment()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(isolationBuilder => isolationBuilder.UseMultiEnvironment(),
            dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiEnvironmentSetter>());
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseMultiEnvironment()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(
            isolationBuilder => isolationBuilder.UseMultiEnvironment().UseMultiEnvironment(),
            dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IMultiEnvironmentContext>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IMultiEnvironmentSetter>().Count() == 1);
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseTenant()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(isolationBuilder => isolationBuilder.UseMultiTenant(),
            dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantContext>());
        Assert.IsNotNull(serviceProvider.GetService<IMultiTenantSetter>());
    }

    [TestMethod]
    public void TestUseIsolationUoWByUseMultiTenant()
    {
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(isolationBuilder => isolationBuilder.UseMultiTenant().UseMultiTenant(),
            dbOptionBuilder => dbOptionBuilder.UseSqlite(_connectionString));

        var serviceProvider = dispatcherOption.Object.Services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IMultiTenantContext>().Count() == 1);
        Assert.IsTrue(serviceProvider.GetServices<IMultiTenantSetter>().Count() == 1);
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
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(
            isolationBuilder => isolationBuilder.UseMultiTenant().UseMultiEnvironment(), dbOptionBuilder => dbOptionBuilder.UseSqlite());
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

        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiTenantContext>().CurrentTenant == null);

        Assert.IsTrue(string.IsNullOrEmpty(unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment));

        unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiTenantContext>().CurrentTenant!.Id ==
            "00000000-0000-0000-0000-000000000002");
        unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("dev");

        Assert.IsTrue(unifOfWorkNew.ServiceProvider.GetRequiredService<IMultiEnvironmentContext>().CurrentEnvironment == "dev");

        var dbContext = unifOfWorkNew.ServiceProvider.GetRequiredService<CustomDbContext>();

        Assert.IsTrue(GetDataBaseConnectionString(dbContext) == "data source=test1" &&
            unitOfWorkAccessorNew.CurrentDbContextOptions!.ConnectionString == "data source=test1");

        var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("development");
        var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test2" &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test2");

        var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew3.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("development");
        var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test2" &&
            unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test2");

        var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
        var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(new Tenant("00000000-0000-0000-0000-000000000002"));
        unifOfWorkNew4.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("production");
        var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test3" &&
            unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test3");
    }

    // [TestMethod]
    // public void TestUseMultiEnvironment()
    // {
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = new ConnectionStrings()
    //         {
    //             DefaultConnection = "data source=test4"
    //         };
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 ConnectionString = "data source=test5",
    //                 Environment = "dev"
    //             },
    //             new()
    //             {
    //                 ConnectionString = "data source=test6",
    //                 Environment = "pro"
    //             }
    //         };
    //     });
    //     Mock<IDispatcherOptions> dispatcherOption = new();
    //     dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
    //     dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(isolationBuilder => isolationBuilder.UseMultiEnvironment(),
    //         dbOptionBuilder => dbOptionBuilder.UseSqlite());
    //     var serviceProvider = _services.BuildServiceProvider();
    //     var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
    //     var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
    //     Assert.IsNotNull(currentDbContextOptions);
    //     Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test4");
    //
    //     var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
    //
    //     var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("dev");
    //     var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test5" &&
    //         unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test5");
    //
    //     var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew3.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("pro");
    //     var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test6" &&
    //         unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test6");
    //
    //     var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew4.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("staging");
    //     var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test4" &&
    //         unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test4");
    // }
    //
    // [TestMethod]
    // public void TestUseMultiTenant()
    // {
    //     _services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = new ConnectionStrings()
    //         {
    //             DefaultConnection = "data source=test7"
    //         };
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 ConnectionString = "data source=test8",
    //                 TenantId = "1"
    //             },
    //             new()
    //             {
    //                 ConnectionString = "data source=test9",
    //                 TenantId = "2"
    //             }
    //         };
    //     });
    //     Mock<IDispatcherOptions> dispatcherOption = new();
    //     dispatcherOption.Setup(builder => builder.Services).Returns(_services).Verifiable();
    //     dispatcherOption.Object.UseIsolationUoW<CustomDbContext, int>(isolationBuilder => isolationBuilder.UseMultiTenant(),
    //         dbOptionBuilder => dbOptionBuilder.UseSqlite());
    //     var serviceProvider = _services.BuildServiceProvider();
    //     var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
    //     var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
    //     Assert.IsNotNull(currentDbContextOptions);
    //     Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test7");
    //
    //     var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
    //
    //     var unifOfWorkNew2 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew2.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(new Tenant("1"));
    //     var dbContext2 = unifOfWorkNew2.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext2) == "data source=test8" &&
    //         unitOfWorkAccessorNew2.CurrentDbContextOptions!.ConnectionString == "data source=test8");
    //
    //     var unifOfWorkNew3 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew3.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(new Tenant("2"));
    //     var dbContext3 = unifOfWorkNew3.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext3) == "data source=test9" &&
    //         unitOfWorkAccessorNew3.CurrentDbContextOptions!.ConnectionString == "data source=test9");
    //
    //     var unifOfWorkNew4 = unitOfWorkManager.CreateDbContext(true);
    //     var unitOfWorkAccessorNew4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
    //     unifOfWorkNew4.ServiceProvider.GetRequiredService<IMultiTenantSetter>().SetTenant(null!);
    //     var dbContext4 = unifOfWorkNew4.ServiceProvider.GetRequiredService<CustomDbContext>();
    //     Assert.IsTrue(GetDataBaseConnectionString(dbContext4) == "data source=test7" &&
    //         unitOfWorkAccessorNew4.CurrentDbContextOptions!.ConnectionString == "data source=test7");
    // }

#pragma warning disable CS0618
    [TestMethod]
    public void TestUseMultiTenantAndAddMasaConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddMasaConfiguration();
        Mock<IDispatcherOptions> dispatcherOption = new();
        dispatcherOption.Setup(opt => opt.Services).Returns(builder.Services).Verifiable();
        dispatcherOption.Object.UseIsolationUoW<CustomDbContext, int>(isolationBuilder => isolationBuilder.UseMultiEnvironment(),
            dbOptionBuilder => dbOptionBuilder.UseSqlite());
        var serviceProvider = builder.Services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var unitOfWorkAccessor = serviceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        var currentDbContextOptions = unitOfWorkAccessor.CurrentDbContextOptions;
        Assert.IsNotNull(currentDbContextOptions);
        Assert.IsTrue(currentDbContextOptions.ConnectionString == "data source=test1");
    }
#pragma warning restore CS0618

    // [TestMethod]
    // public async Task TestUseMultiTenantAndUseFilterAsync()
    // {
    //     var services = new ServiceCollection();
    //     services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = new ConnectionStrings()
    //         {
    //             DefaultConnection = $"data source=test_{Guid.NewGuid()}"
    //         };
    //         option.IsolationConnectionStrings = new List<IsolationOptions>
    //         {
    //             new()
    //             {
    //                 ConnectionString = $"data source=test_{Guid.NewGuid()}",
    //                 TenantId = "1"
    //             }
    //         };
    //     });
    //     Mock<IDispatcherOptions> dispatcherOption = new();
    //     dispatcherOption.Setup(opt => opt.Services).Returns(services).Verifiable();
    //     dispatcherOption.Object.UseIsolationUoW<CustomDbContext>(isolationBuilder =>
    //             isolationBuilder.UseMultiTenant(),
    //         dbOptionBuilder => dbOptionBuilder.UseSqlite().UseFilter().UseFilter());
    //
    //     var serviceProvider = services.BuildServiceProvider();
    //     var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
    //     await customDbContext.Database.EnsureCreatedAsync();
    //     var tag = new Tag()
    //     {
    //         Name = "Tom"
    //     };
    //     await customDbContext.Set<Tag>().AddAsync(tag);
    //     await customDbContext.SaveChangesAsync();
    //
    //     Assert.IsTrue(await customDbContext.Set<Tag>().CountAsync() == 1);
    //
    //     tag = await customDbContext.Set<Tag>().FirstOrDefaultAsync(t => t.Id == tag.Id);
    //     Assert.IsNotNull(tag);
    //
    //     customDbContext.Set<Tag>().Remove(tag);
    //     await customDbContext.SaveChangesAsync();
    //     Assert.IsFalse(await customDbContext.Set<Tag>().AnyAsync());
    //
    //     var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
    //     using (dataFilter.Disable<ISoftDelete>())
    //     {
    //         Assert.IsTrue(await customDbContext.Set<Tag>().AnyAsync());
    //         tag = await customDbContext.Set<Tag>().FirstOrDefaultAsync(t => t.Id == tag.Id);
    //         Assert.IsNotNull(tag);
    //     }
    // }

    private static string GetDataBaseConnectionString(CustomDbContext dbContext) => dbContext.Database.GetConnectionString()!;
}
