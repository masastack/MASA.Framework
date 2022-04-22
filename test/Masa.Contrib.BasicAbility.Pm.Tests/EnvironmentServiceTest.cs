namespace Masa.Contrib.BasicAbility.Pm.Tests;

[TestClass]
public class EnvironmentServiceTest
{
    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int Id)
    {
        var data = new EnvironmentDetailModel();

        var requestUri = $"api/v1/env/{Id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default).Result).Returns(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.EnvironmentService.GetAsync(Id);
        callerProvider.Verify(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGet1Async(int Id)
    {
        EnvironmentDetailModel? data = null;

        var requestUri = $"api/v1/env/{Id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<EnvironmentDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.EnvironmentService.GetAsync(Id);
        callerProvider.Verify(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<EnvironmentModel>
        {
            new EnvironmentModel { Id=1, Color="", Name="" }
        };

        var requestUri = $"api/v1/env";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.EnvironmentService.GetListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetList1Async()
    {
        List<EnvironmentModel>? data = null;

        var requestUri = $"api/v1/env";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<EnvironmentModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.EnvironmentService.GetListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }
}
