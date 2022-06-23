// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class UserServiceTest
{
    [TestMethod]
    public async Task TestAddAsync()
    {
        var addUser = new AddUserModel();
        var user = new UserModel();
        var requestUri = $"api/user/addExternal";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<AddUserModel, UserModel>(requestUri, addUser, default)).ReturnsAsync(user).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.AddAsync(addUser);
        callerProvider.Verify(provider => provider.PostAsync<AddUserModel, UserModel>(requestUri, addUser, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListByDepartmentAsync()
    {
        var data = new List<StaffModel>()
        {
            new StaffModel()
        };
        Guid departmentId = Guid.NewGuid();
        var requestUri = $"api/staff/getListByDepartment";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.GetListByDepartmentAsync(departmentId);
        callerProvider.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetListByTeamAsync()
    {
        var data = new List<StaffModel>()
        {
            new StaffModel()
        };
        Guid teamId = Guid.NewGuid();
        var requestUri = $"api/staff/getListByTeam";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.GetListByTeamAsync(teamId);
        callerProvider.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetListByRoleAsync()
    {
        var data = new List<StaffModel>()
        {
            new StaffModel()
        };
        Guid roleId = Guid.NewGuid();
        var requestUri = $"api/staff/getListByRole";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.GetListByRoleAsync(roleId);
        callerProvider.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    [DataRow("account", "123456")]
    public async Task TestValidateCredentialsByAccountAsync(string account, string password)
    {
        var data = false;
        Guid departmentId = Guid.NewGuid();
        var requestUri = $"api/user/validateByAccount";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<object, bool>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.ValidateCredentialsByAccountAsync(account, password);
        callerProvider.Verify(provider => provider.PostAsync<object, bool>(requestUri, It.IsAny<object>(), default), Times.Once);
    }

    [TestMethod]
    [DataRow("authount")]
    public async Task TestFindByAccountAsync(string account)
    {
        var data = new UserModel();
        var requestUri = $"api/user/findByAccount";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.FindByAccountAsync(account);
        callerProvider.Verify(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("https://www.baidu.com/")]
    public async Task TestVisitedAsync(string url)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/user/visit";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<object>(requestUri, new { UserId = userId, Url = url }, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        await userService.VisitedAsync(url);
        callerProvider.Verify(provider => provider.PostAsync<object>(requestUri, It.IsAny<object>(), true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetUserVisitedListAsync()
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<UserVisitedModel>();
        var requestUri = $"api/user/visitedList";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, List<UserVisitedModel>>(requestUri, new { userId = userId }, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(callerProvider.Object, userContext.Object);
        var result = await userService.GetUserVisitedListAsync();
        callerProvider.Verify(provider => provider.GetAsync<object, List<UserVisitedModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}

