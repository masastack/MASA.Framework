namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class TeamServiceTest
{
    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new TeamDetailModel();
        Guid teamId = Guid.NewGuid();
        var requestUri = $"api/team/deatil";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var authClient = new AuthClient(callerProvider.Object);
        var result = await authClient.TeamService.GetDetailAsync(teamId);
        callerProvider.Verify(provider => provider.GetAsync<object, TeamDetailModel>(requestUri, It.IsAny<object>(), default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}

