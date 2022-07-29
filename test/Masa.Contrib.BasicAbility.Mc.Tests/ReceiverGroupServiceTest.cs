// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Tests;

[TestClass]
public class ReceiverGroupServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new ReceiverGroupModel();
        Guid id = Guid.NewGuid();
        var requestUri = $"api/receiver-group/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<ReceiverGroupModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var receiverGroupService = new Mock<ReceiverGroupService>(caller.Object);
        var result = await receiverGroupService.Object.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<ReceiverGroupModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var options = new GetReceiverGroupModel();
        var data = new PaginatedListModel<ReceiverGroupModel>();
        var requestUri = $"api/receiver-group";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<GetReceiverGroupModel, PaginatedListModel<ReceiverGroupModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var receiverGroupService = new Mock<ReceiverGroupService>(caller.Object);
        var result = await receiverGroupService.Object.GetListAsync(options);
        caller.Verify(provider => provider.GetAsync<GetReceiverGroupModel, PaginatedListModel<ReceiverGroupModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}
