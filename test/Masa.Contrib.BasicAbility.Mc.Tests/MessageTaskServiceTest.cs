// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Mc.Tests;

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
    public async Task TestSendOrdinaryMessageAsync()
    {
        var options = new SendOrdinaryMessageModel();
        var requestUri = $"api/message-task/SendOrdinaryMessage";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendOrdinaryMessageAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSendTemplateMessageAsync()
    {
        var options = new SendTemplateMessageModel();
        var requestUri = $"api/message-task/SendTemplateMessage";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(caller.Object);
        await messageTaskService.Object.SendTemplateMessageAsync(options);
        caller.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
