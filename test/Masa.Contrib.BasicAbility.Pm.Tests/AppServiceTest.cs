// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Pm.Tests;

[TestClass]
public class AppServiceTest
{
    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int id)
    {
        var data = new AppDetailModel();

        var requestUri = $"api/v1/app/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<AppDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGet1Async(int id)
    {
        AppDetailModel? data = null;

        var requestUri = $"api/v1/app/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<AppDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<AppDetailModel>
        {
            new AppDetailModel()
        };

        var requestUri = $"api/v1/app";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetList1Async()
    {
        List<AppDetailModel>? data = null;

        var requestUri = $"api/v1/app";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<AppDetailModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    public async Task TestGetListByProjectIdsAsync()
    {
        var data = new List<AppDetailModel>
        {
            new AppDetailModel()
        };
        var projectIds = new List<int> { 1 };

        var requestUri = $"api/v1/projects/app";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetListByProjectIdsAsync(projectIds);
        callerProvider.Verify(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetListByProjectIds1Async()
    {
        List<AppDetailModel>? data = null;
        var projectIds = new List<int> { 1 };

        var requestUri = $"api/v1/projects/app";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(It.IsAny<string>(), projectIds, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetListByProjectIdsAsync(projectIds);
        callerProvider.Verify(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetWithEnvironmentClusterAsync(int id)
    {
        var data = new AppDetailModel();

        var requestUri = $"api/v1/appWhitEnvCluster/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<AppDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetWithEnvironmentClusterAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetWithEnvironmentCluster1Async(int id)
    {
        AppDetailModel? data = null;

        var requestUri = $"api/v1/appWhitEnvCluster/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<AppDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(callerProvider.Object);

        var result = await pmCaching.AppService.GetWithEnvironmentClusterAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }
}
