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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<AddUserModel, UserModel>(requestUri, addUser, default)).ReturnsAsync(user).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.AddAsync(addUser);
        caller.Verify(provider => provider.PostAsync<AddUserModel, UserModel>(requestUri, addUser, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task UpsertAsync()
    {
        var upsertUser = new UpsertUserModel();
        var user = new UserModel();
        var requestUri = $"api/user/upsertExternal";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<UpsertUserModel, UserModel>(requestUri, upsertUser, default)).ReturnsAsync(user).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.UpsertAsync(upsertUser);
        caller.Verify(provider => provider.PostAsync<UpsertUserModel, UserModel>(requestUri, upsertUser, default), Times.Once);
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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetListByDepartmentAsync(departmentId);
        caller.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetListByTeamAsync(teamId);
        caller.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetListByRoleAsync(roleId);
        caller.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetTotalByDepartmentAsync()
    {
        long data = 10;
        Guid departmentId = Guid.NewGuid();
        var requestUri = $"api/staff/getTotalByDepartment";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetTotalByDepartmentAsync(departmentId);
        caller.Verify(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result == data);
    }

    [TestMethod]
    public async Task TestGetTotalByByRoleAsync()
    {
        long data = 10;
        Guid roleId = Guid.NewGuid();
        var requestUri = $"api/staff/getTotalByRole";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetTotalByRoleAsync(roleId);
        caller.Verify(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result == data);
    }

    [TestMethod]
    public async Task TestGetTotalByTeamAsync()
    {
        long data = 10;
        Guid teamId = Guid.NewGuid();
        var requestUri = $"api/staff/getTotalByTeam";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetTotalByTeamAsync(teamId);
        caller.Verify(provider => provider.GetAsync<object, long>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result == data);
    }

    [TestMethod]
    [DataRow("account", "123456")]
    public async Task TestValidateCredentialsByAccountAsync(string account, string password)
    {
        var data = false;
        Guid departmentId = Guid.NewGuid();
        var requestUri = $"api/user/validateByAccount";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<object, bool>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.ValidateCredentialsByAccountAsync(account, password);
        caller.Verify(provider => provider.PostAsync<object, bool>(requestUri, It.IsAny<object>(), default), Times.Once);
    }

    [TestMethod]
    [DataRow("authount")]
    public async Task TestFindByAccountAsync(string account)
    {
        var data = new UserModel();
        var requestUri = $"api/user/findByAccount";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.FindByAccountAsync(account);
        caller.Verify(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("15168440403")]
    public async Task TestFindByPhoneNumberAsync(string phoneNumber)
    {
        var data = new UserModel()
        {
            PhoneNumber = phoneNumber
        };
        var requestUri = $"api/user/findByPhoneNumber";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.FindByPhoneNumberAsync(phoneNumber);
        caller.Verify(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null && result.PhoneNumber == data.PhoneNumber);
    }

    [TestMethod]
    [DataRow("824255786@qq.com")]
    public async Task TestFindByEmailAsync(string email)
    {
        var data = new UserModel();
        var requestUri = $"api/user/findByEmail";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.FindByEmailAsync(email);
        caller.Verify(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null && result.Email == data.Email);
    }

    [TestMethod]
    public async Task TestGetCurrentUserAsync()
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new UserModel();
        var requestUri = $"api/user/findById";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetCurrentUserAsync();
        caller.Verify(provider => provider.GetAsync<object, UserModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetCurrentStaffAsync()
    {
        var userId = Guid.NewGuid();
        var data = new StaffDetailModel();
        var requestUri = $"api/staff/getExternalByUserId";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, StaffDetailModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetCurrentStaffAsync();
        caller.Verify(provider => provider.GetAsync<object, StaffDetailModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("https://www.baidu.com/")]
    public async Task TestVisitedAsync(string url)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/user/visit";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<object>(requestUri, new { UserId = userId, Url = url }, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.VisitedAsync(url);
        caller.Verify(provider => provider.PostAsync<object>(requestUri, It.IsAny<object>(), true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetVisitedListAsync()
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<UserVisitedModel>() {
            new UserVisitedModel
            {
                Name="baidu",
                Url = "https://www.baidu.com/"
            }
        };
        var requestUri = $"api/user/visitedList";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<UserVisitedModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetVisitedListAsync();
        caller.Verify(provider => provider.GetAsync<object, List<UserVisitedModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestUpdatePasswordAsync()
    {
        var user = new UpdateUserPasswordModel
        {
            Id = Guid.NewGuid(),
            NewPassword = "masa123",
            OldPassword = "masa123"
        };
        var requestUri = $"api/user/updatePassword";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, user, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.UpdatePasswordAsync(user);
        caller.Verify(provider => provider.PutAsync(requestUri, user, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestDisableUserAsync()
    {
        var user = new DisableUserModel("account");
        var requestUri = $"api/user/disable";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync<bool>(requestUri, user, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.DisableUserAsync(user);
        caller.Verify(provider => provider.PutAsync<bool>(requestUri, user, default), Times.Once);
    }

    [TestMethod]
    public async Task TestUpdateBasicInfoAsync()
    {
        var user = new UpdateUserBasicInfoModel
        {
            Id = Guid.NewGuid(),
            DisplayName = "test",
            Gender = GenderTypes.Male,
            PhoneNumber = "15168440403"
        };
        var requestUri = $"api/user/updateBasicInfo";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, user, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.UpdateBasicInfoAsync(user);
        caller.Verify(provider => provider.PutAsync(requestUri, user, true, default), Times.Once);
    }

    [TestMethod]
    public async Task GetUserPortraitsAsync()
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var userPortraits = new List<UserPortraitModel> {
            new UserPortraitModel
            {
                Id = Guid.NewGuid(),
                DisplayName = "DisplayName",
                Account = "Account",
                Name = "Name",
                Avatar = "Avatar"
            }
        };
        var requestUri = $"api/user/portraits";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<Guid[], List<UserPortraitModel>>(requestUri, new Guid[] { userId }, default))
            .ReturnsAsync(userPortraits).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var data = await userService.GetUserPortraitsAsync(userId);
        caller.Verify(provider => provider.PostAsync<Guid[], List<UserPortraitModel>>(requestUri, new Guid[] { userId }, default), Times.Once);
        Assert.IsTrue(data.Count == 1);
    }

    [TestMethod]
    [DataRow("masa-auth")]
    public async Task TestIntGetUserSystemDataAsync(string systemId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = 1;
        var requestUri = $"api/user/GetUserSystemData";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, string>(requestUri, It.IsAny<object>(), default))
            .ReturnsAsync(data.ToString()).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetUserSystemDataAsync<int>(systemId);
        Assert.IsTrue(result == 1);
    }

    [TestMethod]
    [DataRow("masa-auth")]
    public async Task TestObjectGetUserSystemDataAsync(string systemId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new SystemData
        {
            Name = "name",
            Value = "value"
        };
        var requestUri = $"api/user/GetUserSystemData";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, string>(requestUri, It.IsAny<object>(), default))
            .ReturnsAsync(JsonSerializer.Serialize(data)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetUserSystemDataAsync<SystemData>(systemId);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    [DataRow("masa-auth")]
    public async Task TestIntSaveUserSystemDataAsync(string systemId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/user/UserSystemData";
        var value = 1;
        var data = new { UserId = userId, SystemId = systemId, Data = JsonSerializer.Serialize(value) };
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<object>(requestUri, data, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.SaveUserSystemDataAsync(systemId, value);
        caller.Verify(provider => provider.PostAsync<object>(requestUri, It.IsAny<object>(), true, default), Times.Once);
    }

    [TestMethod]
    [DataRow("masa-auth")]
    public async Task TestObjectSaveUserSystemDataAsync(string systemId)
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var requestUri = $"api/user/UserSystemData";
        var value = new SystemData
        {
            Name = "name",
            Value = "value"
        };
        var data = new { UserId = userId, SystemId = systemId, Data = JsonSerializer.Serialize(value) };
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<object>(requestUri, data, true, default)).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var userService = new UserService(caller.Object, userContext.Object);
        await userService.SaveUserSystemDataAsync(systemId, value);
        caller.Verify(provider => provider.PostAsync<object>(requestUri, It.IsAny<object>(), true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetListByAccountAsync()
    {
        var data = new List<UserSimpleModel>()
        {
            new UserSimpleModel(Guid.NewGuid(), "account", "displayName")
        };
        var accounts = new List<string> { "account" };
        var requestUri = $"api/user/GetListByAccount";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, List<UserSimpleModel>>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var userService = new UserService(caller.Object, userContext.Object);
        var result = await userService.GetListByAccountAsync(accounts);
        caller.Verify(provider => provider.GetAsync<object, List<UserSimpleModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }
}

class SystemData
{
    public string Name { get; set; }
    public string Value { get; set; }
}
