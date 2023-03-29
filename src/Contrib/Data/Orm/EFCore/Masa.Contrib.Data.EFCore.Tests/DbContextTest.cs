// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DbContextTest : TestBase
{
    [TestMethod]
    public async Task TestAddAsync()
    {
        await using var dbContext = CreateDbContext(true, out _);
        await dbContext.Set<Student>().AddAsync(new Student()
        {
            Id = 1,
            Name = "Jim",
            Age = 18,
            Address = new Address()
            {
                City = "ShangHai",
                Street = "PuDong",
            },
            Hobbies = new List<Hobby>()
            {
                new()
                {
                    Name = "Sing",
                    Description = "loves singing"
                },
                new()
                {
                    Name = "Game",
                    Description = "mobile game"
                }
            }
        });
        await dbContext.SaveChangesAsync();
        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);
    }

    [TestMethod]
    public async Task TestSoftDeleteAsync()
    {
        Services.Configure<ConnectionStrings>(options =>
        {
            options.DefaultConnection = $"data source=soft-delete-db-{Guid.NewGuid()}";
        });
        await using var dbContext = CreateDbContext(true, out IServiceProvider serviceProvider);
        var student = new Student()
        {
            Id = 1,
            Name = "Jim",
            Age = 18,
            Address = new Address()
            {
                City = "ShangHai",
                Street = "PuDong",
            },
            Hobbies = new List<Hobby>()
            {
                new()
                {
                    Name = "Sing",
                    Description = "loves singing"
                },
                new()
                {
                    Name = "Game",
                    Description = "mobile game"
                }
            }
        };
        await dbContext.Set<Student>().AddAsync(student);
        await dbContext.SaveChangesAsync();
        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);

        dbContext.Set<Student>().Remove(student);
        await dbContext.SaveChangesAsync();

        Assert.IsFalse(await dbContext.Set<Student>().AnyAsync());

        var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
        using (dataFilter.Disable<ISoftDelete>())
        {
            Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);

            student = (await dbContext.Set<Student>().Include(s => s.Address).FirstOrDefaultAsync())!;
            Assert.IsTrue(student.Id == 1);
            Assert.IsTrue(student.Name == "Jim");
            Assert.IsTrue(student.Age == 18);
            Assert.IsTrue(student.IsDeleted);
            Assert.IsTrue(student.Address.City == "ShangHai");
            Assert.IsTrue(student.Address.Street == "PuDong");

            Assert.IsTrue(student.Hobbies.Count == 2);
            Assert.IsTrue(student.Hobbies.Any(h => h.Name == "Sing"));
            Assert.IsTrue(student.Hobbies.Any(h => h.Name == "Game"));
        }
    }

    [TestMethod]
    public async Task TestAddMultiDbContextAsync()
    {
        var services = new ServiceCollection();
        string connectionString = $"data source=test-{Guid.NewGuid()}";
        string connectionStringByQuery = connectionString;
        services.AddMasaDbContext<CustomQueryDbContext>(options => { options.UseSqlite(connectionStringByQuery).UseFilter(); options.EnablePluralizingTableName = false; });
        services.AddMasaDbContext<CustomDbContext>(options => { options.UseSqlite(connectionStringByQuery).UseFilter(); options.EnablePluralizingTableName = false; });
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var queryDbContext = serviceProvider.GetRequiredService<CustomQueryDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await queryDbContext.Database.EnsureCreatedAsync();

        var student = new Student()
        {
            Id = 1,
            Name = "Jim",
            Age = 18,
            Address = new Address()
            {
                City = "ShangHai",
                Street = "PuDong",
            },
            Hobbies = new List<Hobby>()
            {
                new()
                {
                    Name = "Sing",
                    Description = "loves singing"
                },
                new()
                {
                    Name = "Game",
                    Description = "mobile game"
                }
            }
        };
        await dbContext.Set<Student>().AddAsync(student);
        await dbContext.SaveChangesAsync();
        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);

        Assert.IsTrue(await queryDbContext.Set<Student>().AnyAsync());

        dbContext.Remove(student);
        await dbContext.SaveChangesAsync();

        Assert.IsFalse(await dbContext.Set<Student>().AnyAsync());
        Assert.IsFalse(await queryDbContext.Set<Student>().AnyAsync());

        var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
        using (dataFilter.Disable<ISoftDelete>())
        {
            Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);
            Assert.IsTrue(await queryDbContext.Set<Student>().CountAsync() == 1);

            student = (await dbContext.Set<Student>().Include(s => s.Address).FirstOrDefaultAsync())!;
            Assert.IsTrue(student.Id == 1);
            Assert.IsTrue(student.Name == "Jim");
            Assert.IsTrue(student.Age == 18);
            Assert.IsTrue(student.IsDeleted);
            Assert.IsTrue(student.Address.City == "ShangHai");
            Assert.IsTrue(student.Address.Street == "PuDong");

            Assert.IsTrue(student.Hobbies.Count == 2);
            Assert.IsTrue(student.Hobbies.Any(h => h.Name == "Sing"));
            Assert.IsTrue(student.Hobbies.Any(h => h.Name == "Game"));
        }
    }

    [TestMethod]
    public async Task TestDisabledSoftDelete()
    {
        Services.AddMasaDbContext<CustomDbContext>(options
            =>
        { options.UseSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseFilter(); options.EnablePluralizingTableName = false; });
        var serviceProvider = Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var student = new Student
        {
            Id = 1,
            Name = "Jim",
            Age = 18,
            Address = new Address()
            {
                City = "ShangHai",
                Street = "PuDong",
            }
        };
        await dbContext.Set<Student>().AddAsync(student);
        await dbContext.SaveChangesAsync();
        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 1);

        dbContext.Remove(student);
        await dbContext.SaveChangesAsync();

        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 0);

        var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
        using (dataFilter.Disable<ISoftDelete>())
        {
            var count = await dbContext.Set<Student>().CountAsync();
            Assert.IsTrue(count == 1);
        }
    }

    [TestMethod]
    public void TestAddMultiMasaDbContextReturnSaveChangeFilterEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()))
            .AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>().Count() == 3);
    }

    [TestMethod]
    public void TestAddMasaDbContextReturnSaveChangeFilterEqual2()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt =>
        {
            opt.UseSqlite(Guid.NewGuid().ToString()).UseFilter();
            opt.EnablePluralizingTableName = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        var filters = serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>();
        Assert.IsTrue(filters.Count() == 3);
    }

    [TestMethod]
    public async Task TestGetPaginatedListAsyncReturnCountEqualResultCount()
    {
        Services.Configure<ConnectionStrings>(options =>
        {
            options.DefaultConnection = $"data source=soft-delete-db-{Guid.NewGuid()}";
        });
        await using var dbContext = CreateDbContext(true, out IServiceProvider serviceProvider);
        var students = new List<Student>()
        {
            new()
            {
                Id = 1,
                Name = "Jim",
                Age = 18,
                Address = new Address()
                {
                    City = "ShangHai",
                    Street = "PuDong",
                }
            },
            new()
            {
                Id = 2,
                Name = "Tom",
                Age = 20,
                Address = new Address()
                {
                    City = "ShangHai",
                    Street = "PuDong",
                }
            }
        };
        await dbContext.Set<Student>().AddRangeAsync(students);
        await dbContext.SaveChangesAsync();
        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 2);

        var student = students.First();
        dbContext.Attach(student);
        dbContext.Remove(student);
        await dbContext.SaveChangesAsync();

        var result = await new Repository(dbContext).GetPaginatedListAsync(new PaginatedOptions()
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
    }

    [TestMethod]
    public async Task TestModifyConnectionString()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddMasaDbContext<CustomQueryDbContext>(optionsBuilder => { optionsBuilder.UseSqlite(); optionsBuilder.EnablePluralizingTableName = false; });

        var serviceProvider = services.BuildServiceProvider();

        var connectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();
        Assert.IsNotNull(connectionStringProvider);

        var connectionString = await connectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual("data source=test;", connectionString);

        var rootPath = AppDomain.CurrentDomain.BaseDirectory;

        var expectedNewConnectionString = "data source=test2;";
        var oldContent = await File.ReadAllTextAsync(Path.Combine(rootPath, "appsettings.json"));
        await File.WriteAllTextAsync(Path.Combine(rootPath, "appsettings.json"),
            System.Text.Json.JsonSerializer.Serialize(new
            {
                ConnectionStrings = new
                {
                    DefaultConnection = expectedNewConnectionString
                }
            }));

        await Task.Delay(2000);

        connectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();
        Assert.IsNotNull(connectionStringProvider);

        connectionString = await connectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual("data source=test;", connectionString);

        connectionStringProvider = serviceProvider.CreateScope().ServiceProvider.GetService<IConnectionStringProvider>();
        Assert.IsNotNull(connectionStringProvider);

        connectionString = await connectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual(expectedNewConnectionString, connectionString);

        await File.WriteAllTextAsync(Path.Combine(rootPath, "appsettings.json"), oldContent);
    }

    [TestMethod]
    public async Task TestSetCreatorWhenAddEntityAsync()
    {
        var services = new ServiceCollection();
        var userId = Guid.NewGuid().ToString();
        services.AddSingleton<IUserContext>(_ => new CustomUserContext(userId));
        string connectionString = $"data source=test-{Guid.NewGuid()}";
        services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(connectionString).UseFilter());
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = serviceProvider.CreateScope();
        var customDbContext = scope.ServiceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(customDbContext);
        var order = new Order()
        {
            Name = "masa"
        };
        await customDbContext.Set<Order>().AddAsync(order);
        await customDbContext.SaveChangesAsync();

        var orderTemp = await customDbContext.Set<Order>().FirstOrDefaultAsync();
        Assert.IsNotNull(orderTemp);
        Assert.AreEqual(userId, orderTemp.Creator.ToString());
        Assert.AreEqual(userId, orderTemp.Modifier.ToString());
    }

    [TestMethod]
    public async Task TestSetCreatorWhenAddEntityAndUserIdIsNullAsync()
    {
        var services = new ServiceCollection();
        string? userId = null;
        services.AddSingleton<IUserContext>(_ => new CustomUserContext(userId));
        string connectionString = $"data source=test-{Guid.NewGuid()}";
        services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(connectionString).UseFilter());
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = serviceProvider.CreateScope();
        var customDbContext = scope.ServiceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(customDbContext);
        var order = new Order()
        {
            Name = "masa"
        };
        await customDbContext.Set<Order>().AddAsync(order);
        await customDbContext.SaveChangesAsync();

        var orderTemp = await customDbContext.Set<Order>().FirstOrDefaultAsync();
        Assert.IsNotNull(orderTemp);
        Assert.IsNull(orderTemp.Creator);
        Assert.IsNull(orderTemp.Modifier);
    }

    [TestMethod]
    public async Task TestSetCreatorWhenAddEntityAndUserIdIsIntAsync()
    {
        var services = new ServiceCollection();
        services.Configure<AuditEntityOptions>(options => options.UserIdType = typeof(int));
        var userId = "1";
        services.AddSingleton<IUserContext>(_ => new CustomUserContext(userId));
        string connectionString = $"data source=test-{Guid.NewGuid()}";
        services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(connectionString).UseFilter());
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = serviceProvider.CreateScope();
        var customDbContext = scope.ServiceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(customDbContext);
        var goods = new Goods()
        {
            Name = "masa"
        };
        await customDbContext.Set<Goods>().AddAsync(goods);
        await customDbContext.SaveChangesAsync();

        var goodsTemp = await customDbContext.Set<Goods>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsTemp);
        Assert.AreEqual(userId, goodsTemp.Creator.ToString());
        Assert.AreEqual(userId, goodsTemp.Modifier.ToString());
    }
}
