// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Ldap.Novell.Tests;

[TestClass]
public class LdapTest
{
    readonly IServiceCollection Services;
    readonly ILdapProvider ldapProvider;
    readonly ILdapFactory ldapFactory;

    public LdapTest()
    {
        Services = new ServiceCollection();
        var mockLdapOptions = new Mock<LdapOptions>();

        var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();
        var ldapConfigurationSection = configuration.GetSection(nameof(LdapOptions));
        Services.AddLadpContext(ldapConfigurationSection);
        Services.AddLadpContext();
        var serviceProvider = Services.BuildServiceProvider();
        ldapProvider = serviceProvider.GetRequiredService<ILdapProvider>();
        ldapFactory = serviceProvider.GetRequiredService<ILdapFactory>();
    }

    [TestInitialize]
    public void EdgeDriverInitialize()
    {

    }

    [TestMethod]
    public void CreateLdapProvider()
    {
        var ldapProvider = ldapFactory.CreateProvider(new LdapOptions
        {

        });
        Assert.IsNotNull(ldapProvider);
    }

    [TestMethod]
    public async Task GetAllUser()
    {
        var allUsers = ldapProvider.GetAllUserAsync().GetAsyncEnumerator();
        await allUsers.MoveNextAsync();
        Assert.IsNotNull(allUsers.Current);
    }

    [TestMethod]
    public async Task GetPagingUser()
    {
        var pagingUsers = await ldapProvider.GetPagingUserAsync(1);
        Assert.IsTrue(pagingUsers.Count > 0);
    }

    [TestMethod]
    public async Task GetUserByUserName()
    {
        var user = await ldapProvider.GetUserByUserNameAsync("mayue");
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public async Task GetUserByUserEmail()
    {
        var user = await ldapProvider.GetUsersByEmailAddressAsync("mayue@masastack.com");
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public async Task GetGroupAsync()
    {
        var group = await ldapProvider.GetGroupAsync("杭州产品研发部");
        Assert.IsNotNull(group);
    }

    [TestMethod]
    public async Task GetUsersInGroupAsync()
    {
        var users = ldapProvider.GetUsersInGroupAsync("杭州产品研发部").GetAsyncEnumerator();
        await users.MoveNextAsync();
        Assert.IsNotNull(users.Current);
    }
}
