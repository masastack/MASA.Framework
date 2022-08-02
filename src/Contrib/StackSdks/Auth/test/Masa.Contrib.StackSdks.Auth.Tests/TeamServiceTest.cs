// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Auth.Service;

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class TeamServiceTest
{
    [TestMethod]
    public async Task TestGetDetailAsync()
    {
        var data = new TeamDetailModel();
        Guid teamId = Guid.NewGuid();
        var requestUri = $"api/team/detail";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var teamService = new Mock<TeamService>(caller.Object, userContext.Object);
        var result = await teamService.Object.GetDetailAsync(teamId);
        caller.Verify(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetAllAsync()
    {
        var data = new List<TeamModel>();
        var requestUri = $"api/team/list";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<TeamModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        var teamService = new Mock<TeamService>(caller.Object, userContext.Object);
        var result = await teamService.Object.GetAllAsync();
        caller.Verify(provider => provider.GetAsync<List<TeamModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetUserTeamsAsync()
    {
        var userId = Guid.Parse("A9C8E0DD-1E9C-474D-8FE7-8BA9672D53D1");
        var data = new List<TeamModel>();
        var requestUri = $"api/team/list?userId={userId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<TeamModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(user => user.GetUserId<Guid>()).Returns(userId).Verifiable();
        var teamService = new Mock<TeamService>(caller.Object, userContext.Object);
        var result = await teamService.Object.GetUserTeamsAsync();
        caller.Verify(provider => provider.GetAsync<List<TeamModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}

