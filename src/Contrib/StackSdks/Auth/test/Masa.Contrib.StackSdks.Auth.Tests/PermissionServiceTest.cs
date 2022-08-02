// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Auth.Service;

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class PermissionServiceTest
{
    [TestMethod]
    [DataRow("app1")]
    public async Task TestGetMenusAsync(string appId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<MenuModel>();
        var requestUri = $"api/permission/menus?appId={appId}&userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<MenuModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.GetMenusAsync(appId);
        userContext.Verify(user => user.GetUserId<Guid>(), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("app1", "code")]
    public async Task TestAuthorizedAsync(string appId, string code)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = false;
        var requestUri = $"api/permission/authorized?code={code}&userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<bool>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.AuthorizedAsync(appId, code);
        caller.Verify(provider => provider.GetAsync<bool>(It.IsAny<string>(), default), Times.Once);
    }

    [TestMethod]
    [DataRow("app1")]
    public async Task TestGetElementPermissionsAsync(string appId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<string>();
        var requestUri = $"api/permission/element-permissions?appId={appId}&userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<string>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.GetElementPermissionsAsync(appId);
        caller.Verify(provider => provider.GetAsync<List<string>>(It.IsAny<string>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("225082D3-CC88-48D2-3C27-08DA3ED8F4B7")]
    public async Task TestGetFavoriteMenuListAsync(string menuId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<CollectMenuModel>();
        var requestUri = $"api/permission/menu-favorite-list?userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<CollectMenuModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.GetFavoriteMenuListAsync();
        caller.Verify(provider => provider.GetAsync<List<CollectMenuModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("225082D3-CC88-48D2-3C27-08DA3ED8F4B7")]
    public async Task TestAddFavoriteMenuAsync(string menuId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/permission/addFavoriteMenu?permissionId={Guid.Parse(menuId)}&userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, null, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.AddFavoriteMenuAsync(Guid.Parse(menuId));
        caller.Verify(provider => provider.PutAsync(requestUri, null, true, default), Times.Once);
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow("225082D3-CC88-48D2-3C27-08DA3ED8F4B7")]
    public async Task TestRemoveFavoriteMenuAsync(string menuId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/permission/removeFavoriteMenu?permissionId={Guid.Parse(menuId)}&userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, null, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(caller.Object, userContext.Object);
        var result = await permissionService.RemoveFavoriteMenuAsync(Guid.Parse(menuId));
        caller.Verify(provider => provider.PutAsync(requestUri, null, true, default), Times.Once);
        Assert.IsTrue(result);
    }
}
