// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DbContextTest : TestBase
{
    private static string SqliteConnectionString => $"data source=test-{Guid.NewGuid()}";

    #region Test different ways to create context and use

    [TestMethod]
    public void TestDbContext()
    {
        var dbContext = CreateDbContext<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        });
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContext3()
    {
        var dbContext = CreateDbContext<CustomDbContext3>(dbContextBuilder =>
        {
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        });
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContext4()
    {
        var dbContext = CreateDbContext<CustomDbContext4>(dbContextBuilder =>
        {
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        });
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContext5()
    {
        Services.Configure<AppConfigOptions>(options => { options.DbConnectionString = MemoryConnectionString; });
        var dbContext = CreateDbContext<CustomDbContext5>(null);
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContext6()
    {
        var dbContext = CreateDbContext<CustomDbContext6>(dbContextBuilder =>
        {
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        });
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContext7()
    {
        Services.Configure<AppConfigOptions>(options => { options.DbConnectionString = MemoryConnectionString; });
        var dbContext = CreateDbContext<CustomDbContext7>(null);
        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(dbContext.SaveChanges() > 0);
        VerifyStudent(dbContext, student.Id);
    }

    [TestMethod]
    public void TestDbContextWhenNotUseDatabase()
    {
        Assert.ThrowsException<InvalidOperationException>(() => CreateDbContext<CustomDbContextByNotUseDatabase>(null));
    }

    #region Private methods

    private static Student GenerateStudent()
    {
        return new Student
        {
            Id = Guid.NewGuid(),
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
    }

    private static Student VerifyStudent(DbContext dbContext, Guid id, bool isTracking = false)
    {
        var student = isTracking
            ? dbContext.Set<Student>().AsTracking().Include(s => s.Hobbies).FirstOrDefault(s => s.Id == id)
            : dbContext.Set<Student>().AsNoTracking().Include(s => s.Hobbies).FirstOrDefault(s => s.Id == id);
        Assert.IsNotNull(student);
        return student;
    }

    #endregion

    #endregion

    #region Test Filter

    [DataRow(0)]
    [DataRow(1)]
    [DataTestMethod]
    public void TestSaveChangeFilterEqual3(int repeatTimes)
    {
        var services = new ServiceCollection();
        for (int index = 0; index < repeatTimes + 1; index++)
        {
            services.AddMasaDbContext<CustomDbContext>(opt => opt.UseInMemoryDatabase(MemoryConnectionString));
        }

        var serviceProvider = services.BuildServiceProvider();
        Assert.AreEqual(3, serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>().Count());
    }

    /// <summary>
    /// When using tracking, the navigation property cannot get the desired result normally
    /// </summary>
    [TestMethod]
    public async Task TestSoftDeleteAsync()
    {
        IServiceProvider serviceProvider = default!;
        var dbContext = await CreateDbContextAsync<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder.UseFilter();
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        }, sp => serviceProvider = sp);

        var student = GenerateStudent();
        dbContext.Set<Student>().Add(student);
        Assert.IsTrue(await dbContext.SaveChangesAsync() > 0);
        var studentNew = VerifyStudent(dbContext, student.Id, true);
        studentNew.Hobbies.Clear();
        dbContext.Set<Student>().Update(studentNew);
        Assert.IsTrue(await dbContext.SaveChangesAsync() > 0);
        studentNew = VerifyStudent(dbContext, student.Id, true);
        Assert.AreEqual(0, studentNew.Hobbies.Count);

        var dataFilter = serviceProvider.GetRequiredService<IDataFilter>();
        using (dataFilter.Disable<ISoftDelete>())
        {
            studentNew = VerifyStudent(dbContext, student.Id, false);
            Assert.AreEqual(2, studentNew.Hobbies.Count);

            studentNew = VerifyStudent(dbContext, student.Id, true);
            Assert.AreEqual(0, studentNew.Hobbies.Count);
        }

        dbContext.Set<Student>().Remove(studentNew);
        Assert.IsTrue(await dbContext.SaveChangesAsync() > 0);

        studentNew = dbContext.Set<Student>().AsTracking().FirstOrDefault(s => s.Id == student.Id);
        Assert.IsNull(studentNew);

        using (dataFilter.Disable<ISoftDelete>())
        {
            studentNew = VerifyStudent(dbContext, student.Id);
            Assert.AreEqual(2, studentNew.Hobbies.Count);
        }
    }

    [TestMethod]
    public async Task TestModifierBySoftDeleteAsync()
    {
        var creatorUserContext = new CustomUserContext("1");
        Services.Configure<AuditEntityOptions>(options => options.UserIdType = typeof(int));
        Services.AddSingleton<IUserContext>(creatorUserContext);

        IServiceProvider serviceProvider = default!;
        var dbContext = await CreateDbContextAsync<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder.UseFilter();
            dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
        }, sp => serviceProvider = sp);
        var goods = new Goods()
        {
            Name = "masa",
            Logs = new List<OperationLog>()
            {
                new("initialize"),
                new("initialize2"),
            }
        };
        await dbContext.Set<Goods>().AddAsync(goods);
        await dbContext.SaveChangesAsync();

        var goodsCreate = await dbContext.Set<Goods>().Include(g => g.Logs).FirstOrDefaultAsync(g => g.Name == "masa");
        Assert.IsNotNull(goodsCreate);

        var modifierByCrate = goodsCreate.Modifier;
        var modificationTimeByCreate = goodsCreate.ModificationTime;

        Assert.AreEqual(2, goodsCreate.Logs.Count);
        var log1 = goods.Logs.FirstOrDefault(log => log.Name == "initialize");
        Assert.IsNotNull(log1);
        var log2 = goods.Logs.FirstOrDefault(log => log.Name == "initialize2");
        Assert.IsNotNull(log2);

        creatorUserContext.SetUserId("2");

        var inputModificationTime = DateTime.UtcNow.AddDays(-1);
        log1.SetDeleted(true, 3, inputModificationTime);
        goods.Logs.Clear();
        dbContext.Set<Goods>().Update(goods);
        await dbContext.SaveChangesAsync();

        var goodsByUpdate = await dbContext.Set<Goods>().IgnoreQueryFilters().Include(g => g.Logs).FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByUpdate);

        Assert.AreEqual(2, goodsByUpdate.Logs.Count);
        var log1ByUpdate = goodsByUpdate.Logs.FirstOrDefault(log => log.Name == "initialize");
        Assert.IsNotNull(log1ByUpdate);

        Assert.AreEqual(true, log1ByUpdate.IsDeleted);
        Assert.AreEqual(3, log1ByUpdate.Modifier);
        Assert.AreEqual(inputModificationTime, log1ByUpdate.ModificationTime);

        var log1ByUpdate2 = goodsByUpdate.Logs.FirstOrDefault(log => log.Name == "initialize2");
        Assert.IsNotNull(log1ByUpdate2);

        Assert.AreEqual(true, log1ByUpdate2.IsDeleted);
        Assert.AreEqual(2, log1ByUpdate2.Modifier);
        Assert.AreNotEqual(modificationTimeByCreate, log1ByUpdate.ModificationTime);

        var goodsByUpdateAgain = await dbContext.Set<Goods>().AsTracking().IgnoreQueryFilters().Include(g => g.Logs).FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByUpdateAgain);
        var modificationTime = DateTime.Parse("2022-01-01 00:00:00");
        goodsByUpdateAgain.SetDeleted(true, 10, modificationTime);
        dbContext.Set<Goods>().Remove(goodsByUpdateAgain);
        await dbContext.SaveChangesAsync();

        var goodsByDelete = await dbContext.Set<Goods>().AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync(g => g.Name == "masa");
        Assert.IsNotNull(goodsByDelete);

        Assert.AreEqual(10, goodsByDelete.Modifier);
        Assert.AreEqual(modificationTime, goodsByDelete.ModificationTime);
    }

    #endregion

    #region Test ConnectionString

    [TestMethod]
    public async Task TestModifyConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        Services.AddSingleton<IConfiguration>(configuration);

        IServiceProvider serviceProvider = default!;
        var dbContext =
            await CreateDbContextAsync<CustomDbContext>(optionsBuilder => { optionsBuilder.UseSqlite(); }, sp => serviceProvider = sp);

        var connectionStringProvider = serviceProvider.GetService<IConnectionStringProvider>();
        Assert.IsNotNull(connectionStringProvider);

        var connectionString = await connectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual("data source=test;", connectionString);
        Assert.AreEqual("data source=test;", dbContext.Database.GetConnectionString());

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

        await Task.Delay(500);

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

    #endregion

    #region Test Multi DbContext

    [TestMethod]
    public async Task TestUseConfigurationAndSpecify()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        Services.AddSingleton<IConfiguration>(configuration);

        await CreateDbContextAsync<CustomDbContext>(optionsBuilder => { optionsBuilder.UseSqlite(); });
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await CreateDbContextAsync<CustomDbContext2>(optionsBuilder => { optionsBuilder.UseSqlite(SqliteConnectionString); });
        });
    }

    [TestMethod]
    public async Task TestAddMultiDbContextAsync()
    {
        Services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => { dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString); });
        IServiceProvider serviceProvider = default!;
        var customDbContext2 = await CreateDbContextAsync<CustomDbContext2>(
            dbContextBuilder => { dbContextBuilder.UseInMemoryDatabase(MemoryConnectionString); }, sp => serviceProvider = sp);

        var customDbContext = serviceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(customDbContext);

        Assert.IsTrue(customDbContext2 is CustomDbContext2);
        Assert.AreNotEqual(customDbContext, customDbContext2);
    }

    [TestMethod]
    public void TestAddMultiMasaDbContextReturnSaveChangeFilterEqual3()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()))
            .AddMasaDbContext<CustomDbContext>(opt => opt.UseSqlite(Guid.NewGuid().ToString()));

        var serviceProvider = services.BuildServiceProvider();
        Assert.AreEqual(3, serviceProvider.GetServices<ISaveChangesFilter<CustomDbContext>>().Count());
    }

    #endregion

    #region Test Set Creator、Modifier、CreationTime、ModificationTime

    [DataRow(1, 1, 2, 2, 3, 3)]
    [DataRow(null, 0, 2, 2, null, 2)]
    [DataRow(2, 2, null, 2, null, 2)]
    [DataTestMethod]
    public async Task TestAddOrUpdateOrDeleteWhenUserIdIsIntAsync(
        int? creator, int? expectedCreator,
        int? modifier, int? expectedModifier,
        int? deleter, int? expectedDeleter)
    {
        Services.Configure<AuditEntityOptions>(options => options.UserIdType = typeof(int));

        var customUserContext = new CustomUserContext(creator?.ToString());
        Services.AddSingleton<IUserContext>(customUserContext);

        var connectionString = MemoryConnectionString;
        var dbContext = await CreateDbContextAsync<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder
                .UseInMemoryDatabase(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseFilter();
        });
        Assert.IsNotNull(dbContext);
        await dbContext.Set<Goods>().AddAsync(new Goods()
        {
            Name = "masa"
        });
        await dbContext.SaveChangesAsync();

        var goodsByCreate = await dbContext.Set<Goods>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByCreate);
        var creatorByCreate = goodsByCreate.Creator;
        var creationTimeByCreate = goodsByCreate.CreationTime;
        var modifierByCreate = goodsByCreate.Modifier;
        var modificationTimeByCreate = goodsByCreate.ModificationTime;

        Assert.AreEqual(expectedCreator, creatorByCreate);
        Assert.AreEqual(expectedCreator, modifierByCreate);

        customUserContext.SetUserId(modifier?.ToString());
        goodsByCreate.Name = "masa1";
        dbContext.Set<Goods>().Update(goodsByCreate);
        await dbContext.SaveChangesAsync();

        var goodsByUpdate = await dbContext.Set<Goods>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByUpdate);
        var creatorByUpdate = goodsByUpdate.Creator;
        var creationTimeByUpdate = goodsByUpdate.CreationTime;
        var modifierByUpdate = goodsByUpdate.Modifier;
        var modificationTimeByUpdate = goodsByUpdate.ModificationTime;

        Assert.AreEqual(creatorByCreate, creatorByUpdate);
        Assert.AreEqual(creationTimeByCreate, creationTimeByUpdate);
        Assert.AreEqual(expectedModifier, modifierByUpdate);
        Assert.AreNotEqual(modificationTimeByCreate, modificationTimeByUpdate);

        customUserContext.SetUserId(deleter?.ToString());

        dbContext.Set<Goods>().Remove(goodsByUpdate);
        await dbContext.SaveChangesAsync();

        var goodsByDelete = await dbContext.Set<Goods>().IgnoreQueryFilters().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByDelete);
        Assert.AreEqual(creatorByUpdate, goodsByDelete.Creator);
        Assert.AreEqual(creationTimeByUpdate, goodsByDelete.CreationTime);
        Assert.AreEqual(expectedDeleter, goodsByDelete.Modifier);
        Assert.AreNotEqual(modificationTimeByUpdate, goodsByUpdate.ModificationTime);
    }

    [DataRow(1, 1, 2, 2, 3, 3)]
    [DataRow(null, null, 2, 2, null, 2)]
    [DataRow(2, 2, null, 2, null, 2)]
    [DataTestMethod]
    public async Task TestAddOrUpdateOrDeleteWhenUserIdIsNullableIntAsync(
        int? creator, int? expectedCreator,
        int? modifier, int? expectedModifier,
        int? deleter, int? expectedDeleter)
    {
        Services.Configure<AuditEntityOptions>(options => options.UserIdType = typeof(int));

        var customUserContext = new CustomUserContext(creator?.ToString());
        Services.AddSingleton<IUserContext>(customUserContext);

        var connectionString = MemoryConnectionString;
        var dbContext = await CreateDbContextAsync<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder
                .UseInMemoryDatabase(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseFilter();
        });
        Assert.IsNotNull(dbContext);
        await dbContext.Set<Goods2>().AddAsync(new Goods2("masa"));
        await dbContext.SaveChangesAsync();

        var goodsByCreate = await dbContext.Set<Goods2>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByCreate);
        var creatorByCreate = goodsByCreate.Creator;
        var creationTimeByCreate = goodsByCreate.CreationTime;
        var modifierByCreate = goodsByCreate.Modifier;
        var modificationTimeByCreate = goodsByCreate.ModificationTime;
        Assert.AreEqual(expectedCreator, goodsByCreate.Creator);
        Assert.AreEqual(modifierByCreate, goodsByCreate.Modifier);
        Assert.IsTrue((DateTime.UtcNow - creationTimeByCreate).TotalSeconds < 1);
        Assert.IsTrue((DateTime.UtcNow - modificationTimeByCreate).TotalSeconds < 1);

        customUserContext.SetUserId(modifier?.ToString());
        goodsByCreate.Name = "masa1";
        dbContext.Set<Goods2>().Update(goodsByCreate);
        await dbContext.SaveChangesAsync();

        var goodsByUpdate = await dbContext.Set<Goods2>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByUpdate);
        var creatorByUpdate = goodsByUpdate.Creator;
        var creationTimeByUpdate = goodsByUpdate.CreationTime;
        var modifierByUpdate = goodsByUpdate.Modifier;
        var modificationTimeByUpdate = goodsByUpdate.ModificationTime;

        Assert.AreEqual(creatorByCreate, creatorByUpdate);
        Assert.AreEqual(creationTimeByCreate, creationTimeByUpdate);
        Assert.AreEqual(expectedModifier, modifierByUpdate);
        Assert.AreNotEqual(modificationTimeByCreate, modificationTimeByUpdate);
        Assert.IsTrue((DateTime.UtcNow - modificationTimeByUpdate).TotalSeconds < 1);

        customUserContext.SetUserId(deleter?.ToString());

        dbContext.Set<Goods2>().Remove(goodsByUpdate);
        await dbContext.SaveChangesAsync();

        var goodsByDelete = await dbContext.Set<Goods2>().IgnoreQueryFilters().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByDelete);
        Assert.AreEqual(creatorByUpdate, goodsByDelete.Creator);
        Assert.AreEqual(creationTimeByUpdate, goodsByDelete.CreationTime);
        Assert.AreEqual(expectedDeleter, goodsByDelete.Modifier);
        Assert.AreNotEqual(modificationTimeByUpdate, goodsByUpdate.ModificationTime);
        Assert.IsTrue((DateTime.UtcNow - goodsByDelete.ModificationTime).TotalSeconds < 1);
    }

    [DataRow(1, 2, 1, "2023-01-01 00:00:00", "2023-01-01 00:00:00", 3, 3, "2023-01-02 00:00:00", "2023-01-02 00:00:00")]
    [DataTestMethod]
    public async Task TestAddOrUpdateOrDeleteWhenUserIdIsIntAsyncBySpecifyUserIdAndTime(
        int inputCreator, int currentCreator, int expectedCreator,
        string inputCreationTimeStr, string expectedCreationTimeStr,
        int inputModifier, int expectedModifier,
        string inputModificationTimeStr, string expectedModificationTimeStr)
    {
        DateTime inputCreationTime = DateTime.Parse(inputCreationTimeStr), expectedCreationTime = DateTime.Parse(expectedCreationTimeStr);
        DateTime inputModificationTime = DateTime.Parse(inputModificationTimeStr),
            expectedModificationTime = DateTime.Parse(expectedModificationTimeStr);
        Services.Configure<AuditEntityOptions>(options => options.UserIdType = typeof(int));

        var customUserContext = new CustomUserContext(currentCreator.ToString());
        Services.AddSingleton<IUserContext>(customUserContext);

        var connectionString = MemoryConnectionString;
        var dbContext = await CreateDbContextAsync<CustomDbContext>(dbContextBuilder =>
        {
            dbContextBuilder
                .UseInMemoryDatabase(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                .UseFilter();
        });
        Assert.IsNotNull(dbContext);
        var goods = new Goods("masa", inputCreator, inputCreationTime, inputModifier, inputModificationTime);
        await dbContext.Set<Goods>().AddAsync(goods);
        await dbContext.SaveChangesAsync();

        var goodsByCreate = await dbContext.Set<Goods>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByCreate);
        var creatorByCreate = goodsByCreate.Creator;
        var creationTimeByCreate = goodsByCreate.CreationTime;
        var modifierByCreate = goodsByCreate.Modifier;
        var modificationTimeByCreate = goodsByCreate.ModificationTime;
        Assert.AreEqual(expectedCreator, creatorByCreate);
        Assert.AreEqual(expectedCreationTime, creationTimeByCreate);
        Assert.AreEqual(expectedModifier, modifierByCreate);
        Assert.AreEqual(expectedModificationTime, modificationTimeByCreate);

        customUserContext.SetUserId((currentCreator + 5).ToString());
        goodsByCreate.UpdateName("masa1", inputCreator + 2, inputCreationTime.AddDays(1), inputModifier + 3,
            inputModificationTime.AddDays(2));
        dbContext.Set<Goods>().Update(goodsByCreate);
        await dbContext.SaveChangesAsync();

        var goodsByUpdate = await dbContext.Set<Goods>().FirstOrDefaultAsync();
        Assert.IsNotNull(goodsByUpdate);
        var creatorByUpdate = goodsByUpdate.Creator;
        var creationTimeByUpdate = goodsByUpdate.CreationTime;
        var modifierByUpdate = goodsByUpdate.Modifier;
        var modificationTimeByUpdate = goodsByUpdate.ModificationTime;
        Assert.AreEqual(inputCreator + 2, creatorByUpdate);
        Assert.AreEqual(inputCreationTime.AddDays(1), creationTimeByUpdate);
        Assert.AreEqual(currentCreator + 5, modifierByUpdate);
        Assert.AreNotEqual(expectedModificationTime, modificationTimeByUpdate);
        Assert.AreNotEqual(inputModificationTime.AddDays(2), modificationTimeByUpdate);
    }

    #endregion

    #region Test Model Mapping

    [TestMethod]
    public void TestCustomTableName()
    {
        var dbContext = CreateDbContext<CustomDbContext>(dbContext => { dbContext.UseInMemoryDatabase(MemoryConnectionString); });
        var entityTableName = dbContext.Model.FindEntityType(typeof(Student))?.GetTableName();

        Assert.AreEqual("masa_students", entityTableName);
    }

    #endregion

    #region Test QueryTracking

    [TestMethod]
    public void TestQueryTrackingBehaviorByDefault()
    {
        var dbContext = CreateDbContext<CustomDbContext>(dbContext => { dbContext.UseInMemoryDatabase(MemoryConnectionString); });
        Assert.AreEqual(QueryTrackingBehavior.NoTracking, dbContext.ChangeTracker.QueryTrackingBehavior);
    }

    [TestMethod]
    public void TestCustomQueryTrackingBehaviorByConstructor()
    {
        var dbContext = CreateDbContext<CustomDbContextTrackingAll>(dbContext =>
        {
            dbContext.UseInMemoryDatabase(MemoryConnectionString);
        });

        Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
    }

    [TestMethod]
    public void TestCustomQueryTrackingBehaviorByOnConfiguring()
    {
        var dbContext = CreateDbContext<CustomDbContextTrackingAllByOnConfiguring>(null);
        Assert.AreEqual(QueryTrackingBehavior.TrackAll, dbContext.ChangeTracker.QueryTrackingBehavior);
    }

    [TestMethod]
    public void TestQueryTrackingBehaviorByUseQueryTrackingBehavior()
    {
        Services.AddMasaDbContext<CustomDbContext>(masaDbContextBuilder =>
        {
            masaDbContextBuilder.UseInMemoryDatabase(MemoryConnectionString);
            masaDbContextBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });
        var serviceProvider = Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        Assert.AreEqual(QueryTrackingBehavior.NoTrackingWithIdentityResolution, dbContext.ChangeTracker.QueryTrackingBehavior);
    }

    #endregion
}
