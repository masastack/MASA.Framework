// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Tests;

[TestClass]
public class AppServiceTest
{
    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int id)
    {
        var data = new AppDetailModel();

        var requestUri = $"api/v1/app/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGet1Async(int id)
    {
        AppDetailModel? data = null;

        var requestUri = $"api/v1/app/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("identity")]
    public async Task TestGetByIdentityAsync(string identity)
    {
        var data = new AppDetailModel();

        var requestUri = $"open-api/app/{identity}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetByIdentityAsync(identity);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("identity")]
    public async Task TestGetByIdentity1Async(string identity)
    {
        AppDetailModel? data = null;

        var requestUri = $"open-api/app/{identity}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetByIdentityAsync(identity);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetList1Async()
    {
        List<AppDetailModel>? data = null;

        var requestUri = $"api/v1/app";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<AppDetailModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<AppDetailModel>>(requestUri, default), Times.Once);

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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListByProjectIdsAsync(projectIds);
        caller.Verify(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetListByProjectIds1Async()
    {
        List<AppDetailModel>? data = null;
        var projectIds = new List<int> { 1 };

        var requestUri = $"api/v1/projects/app";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(It.IsAny<string>(), projectIds, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListByProjectIdsAsync(projectIds);
        caller.Verify(provider => provider.PostAsync<List<int>, List<AppDetailModel>>(requestUri, projectIds, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetWithEnvironmentClusterAsync(int id)
    {
        var data = new AppDetailModel();

        var requestUri = $"api/v1/appWhitEnvCluster/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetWithEnvironmentClusterAsync(id);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetWithEnvironmentCluster1Async(int id)
    {
        AppDetailModel? data = null;

        var requestUri = $"api/v1/appWhitEnvCluster/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<AppDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetWithEnvironmentClusterAsync(id);
        caller.Verify(provider => provider.GetAsync<AppDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(AppTypes.UI)]
    public async Task TestGetListByAppTypes(params AppTypes[] appTypes)
    {
        var data = new List<AppDetailModel>
        {
            new AppDetailModel()
        };

        var requestUri = $"open-api/app/by-types";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<AppTypes[], List<AppDetailModel>>(requestUri, appTypes, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListByAppTypes(appTypes);
        caller.Verify(provider => provider.PostAsync<AppTypes[], List<AppDetailModel>>(requestUri, appTypes, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    [DataRow(AppTypes.UI)]
    public async Task TestGetListByAppTypes1(params AppTypes[] appTypes)
    {
        List<AppDetailModel>? data = null;

        var requestUri = $"open-api/app/by-types";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<AppTypes[], List<AppDetailModel>>(It.IsAny<string>(), appTypes, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.AppService.GetListByAppTypes(appTypes);
        caller.Verify(provider => provider.PostAsync<AppTypes[], List<AppDetailModel>>(requestUri, appTypes, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }
}
