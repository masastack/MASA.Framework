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
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.UserService.AddAsync(addUser);
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
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.UserService.GetListByDepartmentAsync(departmentId);
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
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.UserService.GetListByTeamAsync(teamId);
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
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.UserService.GetListByRoleAsync(roleId);
        callerProvider.Verify(provider => provider.GetAsync<object, List<StaffModel>>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result.Count == 1);
    }
}

