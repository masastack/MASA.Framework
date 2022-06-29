// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Tests;

[TestClass]
public class ChannelServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new ChannelModel();
        Guid channelId = Guid.NewGuid();
        var requestUri = $"api/channel/{channelId}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<ChannelModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var channelService = new Mock<ChannelService>(callerProvider.Object);
        var result = await channelService.Object.GetAsync(channelId);
        callerProvider.Verify(provider => provider.GetAsync<ChannelModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var options = new GetChannelModel();
        var data = new PaginatedListModel<ChannelModel>();
        var requestUri = $"api/channel";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<GetChannelModel, PaginatedListModel<ChannelModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var channelService = new Mock<ChannelService>(callerProvider.Object);
        var result = await channelService.Object.GetListAsync(options);
        callerProvider.Verify(provider => provider.GetAsync<GetChannelModel, PaginatedListModel<ChannelModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}
