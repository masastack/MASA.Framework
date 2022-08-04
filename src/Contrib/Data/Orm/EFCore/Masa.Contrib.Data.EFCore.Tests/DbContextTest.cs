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
        Services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = $"data source=soft-delete-db-{Guid.NewGuid()}"
            };
        });
        await using var dbContext = CreateDbContext(true, out IServiceProvider serviceProvider, false);
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

        student = await dbContext.Set<Student>().Include(s => s.Address).Include(s => s.Hobbies).FirstAsync();
        dbContext.Set<Student>().Remove(student);
        await dbContext.SaveChangesAsync();

        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 0);

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
    public async Task TestDisabledSoftDelete()
    {
        Services.AddMasaDbContext<CustomDbContext>(options
            => options.UseTestFilter().UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}"));
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

        dbContext.Set<Student>().Remove(student);
        await dbContext.SaveChangesAsync();

        Assert.IsTrue(await dbContext.Set<Student>().CountAsync() == 0);

        var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
        using (dataFilter.Disable<ISoftDelete>())
        {
            var count = await dbContext.Set<Student>().IgnoreQueryFilters().CountAsync();
            Assert.IsTrue(count == 1);
        }
    }

    [TestMethod]
    public void TestAddMultiMasaDbContextReturnSaveChangeFilterEqual1()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>()
            .AddMasaDbContext<CustomDbContext>();

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<ISaveChangesFilter>().Count() == 1);
    }

    [TestMethod]
    public void TestAddMasaDbContextReturnSaveChangeFilterEqual2()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt =>
        {
            opt.UseTestSqlite(Guid.NewGuid().ToString()).UseFilter();
        });

        var serviceProvider = services.BuildServiceProvider();

        var filters = serviceProvider.GetServices<ISaveChangesFilter>();
        Assert.IsTrue(filters.Count() == 2);
    }

    [TestMethod]
    public async Task TestGetPaginatedListAsyncReturnCountEqualResultCount()
    {
        Services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = $"data source=soft-delete-db-{Guid.NewGuid()}"
            };
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

        var student = await dbContext.Set<Student>().FirstAsync();
        dbContext.Set<Student>().Remove(student);
        await dbContext.SaveChangesAsync();

        var result = await new Repository(dbContext).GetPaginatedListAsync(new PaginatedOptions()
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
    }
}
