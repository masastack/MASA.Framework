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
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<MessageTaskModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(callerProvider.Object);
        var result = await messageTaskService.Object.GetAsync(id);
        callerProvider.Verify(provider => provider.GetAsync<MessageTaskModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task TestSendOrdinaryMessageAsync()
    {
        var options = new SendOrdinaryMessageModel();
        var requestUri = $"api/message-task/SendOrdinaryMessage";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(callerProvider.Object);
        await messageTaskService.Object.SendOrdinaryMessageAsync(options);
        callerProvider.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestSendTemplateMessageAsync()
    {
        var options = new SendTemplateMessageModel();
        var requestUri = $"api/message-task/SendTemplateMessage";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.PostAsync(requestUri, options, true, default)).Verifiable();
        var messageTaskService = new Mock<MessageTaskService>(callerProvider.Object);
        await messageTaskService.Object.SendTemplateMessageAsync(options);
        callerProvider.Verify(provider => provider.PostAsync(requestUri, options, true, default), Times.Once);
    }
}
