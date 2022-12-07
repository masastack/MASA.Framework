// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class LinqExtensionsTest : TestBase
{
    private CustomDbContext context;

    [TestInitialize]
    public async Task Init() {
        context = CreateDbContext(true, out _);
        await context.Set<User>().AddAsync(new User()
        {
            Name = "Makani",
            Email = "Makani@163.com",
            PhoneNumber = "15244499326",
            Age = 13,
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
    public async Task TestGetQueryable()
    {
        var user = (await context.Set<User>().GetQueryable(new Dictionary<string, object>
        {
            [nameof(User.Age)] = 18,
            [nameof(User.PhoneNumber)] = "16151288302",
        }).FirstOrDefaultAsync())!;

        Assert.IsTrue(user.Name == "Beaudine");
        Assert.IsTrue(user.Email == "Beaudine@163.com");
        Assert.IsTrue(user.PhoneNumber == "16151288302");
        Assert.IsTrue(user.Age == 18);
        Assert.IsTrue(user.CompanyName == "温州数闪");
    }

    [TestMethod]
    public async Task TestOrderBy()
    {
        var user = (await context.Set<User>().OrderBy(new Dictionary<string, bool>
        {
            [nameof(User.Age)] = false
        }).FirstOrDefaultAsync())!;

        Assert.IsTrue(user.Name == "Makani");
        Assert.IsTrue(user.Email == "Makani@163.com");
        Assert.IsTrue(user.PhoneNumber == "15244499326");
        Assert.IsTrue(user.Age == 13);
        Assert.IsTrue(user.CompanyName == "温州数闪");
    }

    [TestMethod]
    public async Task TestThenBy()
    {
        var user = (await context.Set<User>().OrderBy(new Dictionary<string, bool>
        {
            [nameof(User.CompanyName)] = true,
            [nameof(User.Age)] = true,
        }).FirstOrDefaultAsync())!;

        Assert.IsTrue(user.Name == "Beaudine");
        Assert.IsTrue(user.Email == "Beaudine@163.com");
        Assert.IsTrue(user.PhoneNumber == "16151288302");
        Assert.IsTrue(user.Age == 18);
        Assert.IsTrue(user.CompanyName == "温州数闪");
    }
}
