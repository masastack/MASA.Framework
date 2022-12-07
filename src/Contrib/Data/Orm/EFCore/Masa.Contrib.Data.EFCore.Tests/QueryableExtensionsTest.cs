// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class QueryableExtensionsTest : TestBase
{
    private CustomDbContext context;

    [TestInitialize]
    public async Task Init()
    {
        context = CreateDbContext(true, out _);
        await context.Set<User>().AddAsync(new User()
        {
            Name = "Makani",
            Email = "Makani@163.com",
            PhoneNumber = "15244499326",
            Age = 18,
            CompanyName = "温州数闪",
        });
        await context.Set<User>().AddAsync(new User()
        {
            Name = "Beaudine",
            Email = "Beaudine@163.com",
            PhoneNumber = "16151288302",
            Age = 18,
            CompanyName = "温州数闪",
        });
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task TestGetPaginatedListAsync()
    {
        var result = await context.Set<User>().GetPaginatedListAsync(new BuildingBlocks.Ddd.Domain.Repositories.PaginatedOptions
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
    }

    [TestMethod]
    public async Task TestGetPaginatedListByPredicateAsync()
    {
        var result = await context.Set<User>().GetPaginatedListAsync(x => x.PhoneNumber == "16151288302", new BuildingBlocks.Ddd.Domain.Repositories.PaginatedOptions
        {
            Page = 1,
            PageSize = 10
        });

        Assert.IsTrue(result.Result.Count == result.Total);
        Assert.IsTrue(result.Result.FirstOrDefault()?.Name == "Beaudine");
    }
}
