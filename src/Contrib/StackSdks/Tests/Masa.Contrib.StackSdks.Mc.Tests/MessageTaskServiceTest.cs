// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Mc.Service;

namespace Masa.Contrib.StackSdks.Mc.Tests;

[TestClass]
public class MessageTaskServiceTest
{
    [TestMethod]
    public async Task TestGetAsync()
    {
        var data = new MessageTaskModel();
        Guid id = Guid.NewGuid();
        var requestUri = $"api/message-task/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<MessageTaskModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        var result = await messageTaskService.Object.GetAsync(id);
        caller.Verify(provider => provider.GetAsync<MessageTaskModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestSendOrdinaryMessageByInternalAsync()
    {
        var options = new SendOrdinaryMessageByInternalModel();
        var requestUri = $"api/message-task/SendOrdinaryMessageByInternal";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendOrdinaryMessageByInternalAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSendTemplateMessageByInternalAsync()
    {
        var options = new SendTemplateMessageByInternalModel();
        var requestUri = $"api/message-task/SendTemplateMessageByInternal";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendTemplateMessageByInternalAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSendOrdinaryMessageByExternalAsync()
    {
        var options = new SendOrdinaryMessageByExternalModel();
        var requestUri = $"api/message-task/SendOrdinaryMessageByExternal";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendOrdinaryMessageByExternalAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSendTemplateMessageByExternalAsync()
    {
        var options = new SendTemplateMessageByExternalModel();
        var requestUri = $"api/message-task/SendTemplateMessageByExternal";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendTemplateMessageByExternalAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
