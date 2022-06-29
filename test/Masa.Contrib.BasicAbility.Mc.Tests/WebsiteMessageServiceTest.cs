// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Tests;

[TestClass]
public class WebsiteMessageServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new WebsiteMessageModel();
        Guid id = Guid.NewGuid();
        var requestUri = $"api/website-message/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<WebsiteMessageModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        var result = await websiteMessageService.Object.GetAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<WebsiteMessageModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var options = new GetWebsiteMessageModel();
        var data = new PaginatedList<WebsiteMessageModel>();
        var requestUri = $"api/website-message";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<GetWebsiteMessageModel, PaginatedList<WebsiteMessageModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        var result = await websiteMessageService.Object.GetListAsync(options);
        callerProvider.Verify(provider => provider.GetAsync<GetWebsiteMessageModel, PaginatedList<WebsiteMessageModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestCheckAsync()
    {
        var requestUri = $"api/website-message/Check";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync(requestUri, null, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        await websiteMessageService.Object.CheckAsync();
        callerProvider.Verify(provider => provider.PostAsync(requestUri, null, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestDeleteAsync()
    {
        Guid id = Guid.NewGuid();
        var requestUri = $"api/website-message/{id}";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.DeleteAsync(requestUri, null, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        await websiteMessageService.Object.DeleteAsync(id);
        callerProvider.Verify(provider => provider.DeleteAsync(requestUri, null, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetChannelListAsync()
    {
        var data = new List<WebsiteMessageChannelModel>();
        var requestUri = $"api/website-message/GetChannelList";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<List<WebsiteMessageChannelModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        var result = await websiteMessageService.Object.GetChannelListAsync();
        callerProvider.Verify(provider => provider.GetAsync<List<WebsiteMessageChannelModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetNoticeListAsync()
    {
        var options = new GetNoticeListModel();
        var data = new List<WebsiteMessageModel>();
        var requestUri = $"api/website-message/GetNoticeList";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        var result = await websiteMessageService.Object.GetNoticeListAsync(options);
        callerProvider.Verify(provider => provider.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestReadAsync()
    {
        var options = new ReadWebsiteMessageModel();
        var requestUri = $"api/website-message/Read";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        await websiteMessageService.Object.ReadAsync(options);
        callerProvider.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSetAllReadAsync()
    {
        var options = new ReadAllWebsiteMessageModel();
        var requestUri = $"api/website-message/SetAllRead";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(callerProvider.Object);
        await websiteMessageService.Object.SetAllReadAsync(options);
        callerProvider.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
