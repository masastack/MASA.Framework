// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Tests;

[TestClass]
public class ClusterServiceTest
{
    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int id)
    {
        var data = new ClusterDetailModel();

        var requestUri = $"api/v1/cluster/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<ClusterDetailModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<ClusterDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync1(int id)
    {
        ClusterDetailModel? data = null;

        var requestUri = $"api/v1/cluster/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<ClusterDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<ClusterDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TestGetEnvironmentClustersAsync()
    {
        var data = new List<EnvironmentClusterModel>
        {
            new EnvironmentClusterModel { Id = 1 }
        };

        var requestUri = $"api/v1/envClusters";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<EnvironmentClusterModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetEnvironmentClustersAsync();
        caller.Verify(provider => provider.GetAsync<List<EnvironmentClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetEnvironmentClusters1Async()
    {
        List<EnvironmentClusterModel>? data = null;

        var requestUri = $"api/v1/envClusters";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<EnvironmentClusterModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetEnvironmentClustersAsync();
        caller.Verify(provider => provider.GetAsync<List<EnvironmentClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var data = new List<ClusterModel>
        {
            new ClusterModel { Id = 1 }
        };

        var requestUri = $"api/v1/cluster";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetList1Async()
    {
        List<ClusterModel>? data = null;

        var requestUri = $"api/v1/cluster";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ClusterModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetListByEnvIdAsync(int envId)
    {
        var data = new List<ClusterModel>
        {
            new ClusterModel { Id = 1 }
        };

        var requestUri = $"api/v1/{envId}/cluster";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetListByEnvIdAsync(envId);
        caller.Verify(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGetListByEnvId1Async(int envId)
    {
        List<ClusterModel>? data = null;

        var requestUri = $"api/v1/{envId}/cluster";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ClusterModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.ClusterService.GetListByEnvIdAsync(envId);
        caller.Verify(provider => provider.GetAsync<List<ClusterModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }
}
