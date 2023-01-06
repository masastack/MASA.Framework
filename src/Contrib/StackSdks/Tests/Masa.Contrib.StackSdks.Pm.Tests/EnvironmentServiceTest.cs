// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Tests;

[TestClass]
public class EnvironmentServiceTest
{
    [TestMethod]
    [DataRow(1)]
    public async Task TestGetAsync(int id)
    {
        var data = new EnvironmentDetailModel();

        var requestUri = $"api/v1/env/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default).Result).Returns(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.EnvironmentService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow(1)]
    public async Task TestGet1Async(int id)
    {
        EnvironmentDetailModel? data = null;

        var requestUri = $"api/v1/env/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<EnvironmentDetailModel>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.EnvironmentService.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<EnvironmentDetailModel>(requestUri, default), Times.Once);

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
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.EnvironmentService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 1);
    }

    [TestMethod]
    public async Task TestGetList1Async()
    {
        List<EnvironmentModel>? data = null;

        var requestUri = $"api/v1/env";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<EnvironmentModel>>(It.IsAny<string>(), default)).ReturnsAsync(data).Verifiable();
        var pmCaching = new PmClient(caller.Object);

        var result = await pmCaching.EnvironmentService.GetListAsync();
        caller.Verify(provider => provider.GetAsync<List<EnvironmentModel>>(requestUri, default), Times.Once);

        Assert.IsTrue(result.Count == 0);
    }
}
