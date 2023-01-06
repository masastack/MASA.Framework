// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Tests;

[TestClass]
public class WebsiteMessageServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new WebsiteMessageModel();
        Guid id = Guid.NewGuid();
        var requestUri = $"api/website-message/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<WebsiteMessageModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        var result = await websiteMessageService.Object.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<WebsiteMessageModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetListAsync()
    {
        var options = new GetWebsiteMessageModel(1,10);
        var data = new PaginatedListModel<WebsiteMessageModel>();
        var requestUri = $"api/website-message";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<GetWebsiteMessageModel, PaginatedListModel<WebsiteMessageModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        var result = await websiteMessageService.Object.GetListAsync(options);
        caller.Verify(provider => provider.GetAsync<GetWebsiteMessageModel, PaginatedListModel<WebsiteMessageModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestCheckAsync()
    {
        var requestUri = $"api/website-message/Check";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, null, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        await websiteMessageService.Object.CheckAsync();
        caller.Verify(provider => provider.PostAsync(requestUri, null, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestDeleteAsync()
    {
        Guid id = Guid.NewGuid();
        var requestUri = $"api/website-message/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.DeleteAsync(requestUri, null, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        await websiteMessageService.Object.DeleteAsync(id);
        caller.Verify(provider => provider.DeleteAsync(requestUri, null, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetChannelListAsync()
    {
        var data = new List<WebsiteMessageChannelModel>();
        var requestUri = $"api/website-message/GetChannelList";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<WebsiteMessageChannelModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        var result = await websiteMessageService.Object.GetChannelListAsync();
        caller.Verify(provider => provider.GetAsync<List<WebsiteMessageChannelModel>>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestGetNoticeListAsync()
    {
        var options = new GetNoticeListModel();
        var data = new List<WebsiteMessageModel>();
        var requestUri = $"api/website-message/GetNoticeList";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options, default)).ReturnsAsync(data).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        var result = await websiteMessageService.Object.GetNoticeListAsync(options);
        caller.Verify(provider => provider.GetAsync<GetNoticeListModel, List<WebsiteMessageModel>>(requestUri, options, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestReadAsync()
    {
        var options = new ReadWebsiteMessageModel();
        var requestUri = $"api/website-message/Read";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        await websiteMessageService.Object.ReadAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSetAllReadAsync()
    {
        var options = new ReadAllWebsiteMessageModel(1,10);
        var requestUri = $"api/website-message/SetAllRead";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var websiteMessageService = new Mock<WebsiteMessageService>(caller.Object);
        await websiteMessageService.Object.SetAllReadAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
