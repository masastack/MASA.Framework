// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Tests;

[TestClass]
public class ChannelServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new ChannelModel();
        Guid channelId = Guid.NewGuid();
        var requestUri = $"api/channel/{channelId}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<ChannelModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var channelService = new Mock<ChannelService>(caller.Object);
        var result = await channelService.Object.GetAsync(channelId);
        caller.Verify(provider => provider.GetAsync<ChannelModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var options = new GetChannelModel();
        var data = new PaginatedListModel<ChannelModel>();
        var requestUri = $"api/channel";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<GetChannelModel, PaginatedListModel<ChannelModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var channelService = new Mock<ChannelService>(caller.Object);
        var result = await channelService.Object.GetListAsync(options);
        caller.Verify(provider => provider.GetAsync<GetChannelModel, PaginatedListModel<ChannelModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestCreateAsync()
    {
        var options = new CreateChannelModel();
        var requestUri = $"api/channel";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<ChannelService>(caller.Object);
        await messageTaskService.Object.CreateAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
