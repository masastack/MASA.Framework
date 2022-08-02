// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Dcc.Tests;

[TestClass]
public class LabelServiceTest
{
    [TestMethod]
    [DataRow("status")]
    public async Task TestGetListByTypeCodeAsync(string typeCode)
    {
        var data = new List<LabelModel>
        {
            new LabelModel { Code = "normal" },
            new LabelModel { Code = "frozen" }
        };

        var distributedCacheClient = new Mock<IDistributedCacheClient>();
        distributedCacheClient.Setup(client => client.GetAsync<List<LabelModel>>(typeCode)).ReturnsAsync(data).Verifiable();
        var dccClient = new DccClient(distributedCacheClient.Object);

        var result = await dccClient.LabelService.GetListByTypeCodeAsync(typeCode);
        distributedCacheClient.Verify(client => client.GetAsync<List<LabelModel>>(typeCode), Times.Once);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    [DataRow("status")]
    public async Task TestGetListByTypeCodeAsync2(string typeCode)
    {
        List<LabelModel>? data = null;

        var distributedCacheClient = new Mock<IDistributedCacheClient>();
        distributedCacheClient.Setup(client => client.GetAsync<List<LabelModel>>(It.IsAny<string>())).ReturnsAsync(data).Verifiable();
        var dccClient = new DccClient(distributedCacheClient.Object);

        var result = await dccClient.LabelService.GetListByTypeCodeAsync(typeCode);
        distributedCacheClient.Verify(client => client.GetAsync<List<LabelModel>>(typeCode), Times.Once);

        Assert.IsFalse(result.Any());
    }
}
