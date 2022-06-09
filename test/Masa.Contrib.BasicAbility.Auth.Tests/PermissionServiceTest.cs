// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class PermissionServiceTest : BaseAuthTest
{
    [TestMethod]
    [DataRow("app1", "A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1")]
    public async Task TestGetMenusAsync(string appId, string userId)
    {
        var data = new List<MenuModel>();
        var requestUri = $"api/permission/menus?appId={appId}&userId={userId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<MenuModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.PermissionService.GetMenusAsync(appId, Guid.Parse(userId));
        callerProvider.Verify(provider => provider.GetAsync<List<MenuModel>>(It.IsAny<string>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("app1", "code", "A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1")]
    public async Task TestAuthorizedAsync(string appId, string code, string userId)
    {
        var data = false;
        var requestUri = $"api/permission/authorized?code={code}&userId={userId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<bool>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.PermissionService.AuthorizedAsync(appId, code, Guid.Parse(userId));
        callerProvider.Verify(provider => provider.GetAsync<bool>(It.IsAny<string>(), default), Times.Once);
    }

    [TestMethod]
    [DataRow("app1", "A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1")]
    public async Task TestGetElementPermissionsAsync(string appId, string userId)
    {
        var data = new List<string>();
        var requestUri = $"api/permission/element-permissions?appId={appId}&userId={userId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<string>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.PermissionService.GetElementPermissionsAsync(appId, Guid.Parse(userId));
        callerProvider.Verify(provider => provider.GetAsync<List<string>>(It.IsAny<string>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}
