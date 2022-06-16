// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Identity.IdentityModel;
using Masa.Contrib.BasicAbility.Auth.Service;

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class PermissionServiceTest : BaseAuthTest
{
    [TestMethod]
    [DataRow("app1")]
    public async Task TestGetMenusAsync(string appId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<MenuModel>();
        var requestUri = $"api/permission/menus?appId={appId}&userId={userId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<MenuModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(callerProvider.Object, userContext.Object);
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
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<bool>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(callerProvider.Object, userContext.Object);
        var result = await permissionService.AuthorizedAsync(appId, code);
        callerProvider.Verify(provider => provider.GetAsync<bool>(It.IsAny<string>(), default), Times.Once);
    }

    [TestMethod]
    [DataRow("app1")]
    public async Task TestGetElementPermissionsAsync(string appId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<string>();
        var requestUri = $"api/permission/element-permissions?appId={appId}&userId={userId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<string>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var permissionService = new PermissionService(callerProvider.Object, userContext.Object);
        var result = await permissionService.GetElementPermissionsAsync(appId);
        callerProvider.Verify(provider => provider.GetAsync<List<string>>(It.IsAny<string>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}
