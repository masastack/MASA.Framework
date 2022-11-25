// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.Tests;

[TestClass]
public class DataFilterTest
{
    private IServiceCollection _services;
    private IDataFilter _dataFilter;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddSingleton<IDataFilter, DataFilter>();
        _services.AddSingleton(typeof(DataFilter<>));
        _dataFilter = new DataFilter(_services.BuildServiceProvider());
    }

    [TestMethod]
    public void TestDataFilterReturnTrue()
    {
        Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Disable<ISoftDelete>())
        {
            Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [TestMethod]
    public void TestDataFilterReturnFalse()
    {
        _dataFilter.Disable<ISoftDelete>();
        Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());

        using (_dataFilter.Enable<ISoftDelete>())
        {
            Assert.IsTrue(_dataFilter.IsEnabled<ISoftDelete>());
        }

        Assert.IsFalse(_dataFilter.IsEnabled<ISoftDelete>());
    }

    [TestMethod]
    public async Task TestSoftDeleteAsync()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(options =>
        {
            options.UseTestSqlite($"data source=disabled-soft-delete-db-{Guid.NewGuid()}").UseFilter();
        });
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        DateTime createTime = DateTime.Now;
        var student = new Student()
        {
            Id = 1,
            Name = "Name",
            Age = 18,
            Address = new Address()
            {
                City = "city",
                Street = "street",
                LastLog = new LogItem()
                {
                    Level = (int)LogLevel.Information,
                    Message = "Add Student",
                    CreateTime = createTime
                }
            },
            Hobbies = new List<Hobby>()
            {
                new()
                {
                    Name = "Hobby.Name",
                    Description = "Hobby.Description"
                }
            }
        };
        dbContext.Set<Student>().Add(student);
        await dbContext.SaveChangesAsync();

        student = dbContext.Set<Student>().Include(s => s.Address).FirstOrDefault(s => s.Id == 1);
        Assert.IsNotNull(student);

        var creationTime = student.CreationTime;
        var modificationTime = student.ModificationTime;

        await Task.Delay(1000);

        dbContext.Set<Student>().Remove(student);
        var row = await dbContext.SaveChangesAsync();
        Assert.IsTrue(row > 0);

        var newStudent = dbContext.Set<Student>().IgnoreQueryFilters().FirstOrDefault(s => s.Id == student.Id);
        Assert.IsNotNull(newStudent);

        Assert.AreEqual(creationTime, newStudent.CreationTime);
        Assert.AreNotEqual(modificationTime, newStudent.ModificationTime);
        Assert.AreEqual(createTime, newStudent.Address.LastLog.CreateTime);
    }
}
