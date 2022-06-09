// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class TeamServiceTest : BaseAuthTest
{
    [TestMethod]
    public async Task TestGetDetailAsync()
    {
        var data = new TeamDetailModel();
        Guid teamId = Guid.NewGuid();
        var requestUri = $"api/team/detail";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.TeamService.GetDetailAsync(teamId);
        callerProvider.Verify(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<TeamModel>();
        var requestUri = $"api/team/list";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<TeamModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.TeamService.GetListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<TeamModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}

