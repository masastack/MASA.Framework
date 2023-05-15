// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class LinqAndQueryableExtensionsTest : TestBase
{
    private DbContext _context;

    [TestInitialize]
    public async Task InitializeData()
    {
        _context = await CreateDbContextAsync<CustomDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseInMemoryDatabase(MemoryConnectionString).UseFilter();
        });
        await _context.Set<User>().AddAsync(new User()
        {
            Name = "Makani",
            Email = "Makani@163.com",
            PhoneNumber = "15244499326",
            Age = 13,
            CompanyName = "masa stack",
        });
        await _context.Set<User>().AddAsync(new User()
        {
            Name = "Beaudine",
            Email = "Beaudine@163.com",
            PhoneNumber = "16151288302",
            Age = 18,
            CompanyName = "masa stack",
        });
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task TestExtensionMethodsAsync()
    {
        TestGetQueryable();
        TestOrderBy();
        TestThenBy();
        await TestGetPaginatedListAsync();
        await TestGetPaginatedListByPredicateAsync();
    }

    private void TestGetQueryable()
    {
        var user = _context.Set<User>().GetQueryable(new Dictionary<string, object>
        {
            [nameof(User.Age)] = 18,
            [nameof(User.PhoneNumber)] = "16151288302",
        }).FirstOrDefault();
        Assert.IsNotNull(user);

        Assert.IsTrue(user.Name == "Beaudine");
        Assert.IsTrue(user.Email == "Beaudine@163.com");
        Assert.IsTrue(user.PhoneNumber == "16151288302");
        Assert.IsTrue(user.Age == 18);
        Assert.IsTrue(user.CompanyName == "masa stack");
    }

    private void TestOrderBy()
    {
        var user = _context.Set<User>().OrderBy(new Dictionary<string, bool>
        {
            [nameof(User.Age)] = false
        }).FirstOrDefault();
        Assert.IsNotNull(user);

        Assert.IsTrue(user.Name == "Makani");
        Assert.IsTrue(user.Email == "Makani@163.com");
        Assert.IsTrue(user.PhoneNumber == "15244499326");
        Assert.IsTrue(user.Age == 13);
        Assert.IsTrue(user.CompanyName == "masa stack");
    }

    private void TestThenBy()
    {
        var user = ( _context.Set<User>().OrderBy(new Dictionary<string, bool>
        {
            [nameof(User.CompanyName)] = true,
            [nameof(User.Age)] = true,
        }).FirstOrDefault());
        Assert.IsNotNull(user);

        Assert.IsTrue(user.Name == "Beaudine");
        Assert.IsTrue(user.Email == "Beaudine@163.com");
        Assert.IsTrue(user.PhoneNumber == "16151288302");
        Assert.IsTrue(user.Age == 18);
        Assert.IsTrue(user.CompanyName == "masa stack");
    }

    private async Task TestGetPaginatedListAsync()
    {
        var result = await _context.Set<User>().GetPaginatedListAsync(new BuildingBlocks.Ddd.Domain.Repositories.PaginatedOptions
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
    }

    private async Task TestGetPaginatedListByPredicateAsync()
    {
        var result = await _context.Set<User>().GetPaginatedListAsync(x => x.PhoneNumber == "16151288302", new BuildingBlocks.Ddd.Domain.Repositories.PaginatedOptions
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
        Assert.IsTrue(result.Result.FirstOrDefault()?.Name == "Beaudine");
    }
}
