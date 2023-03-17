// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Alert.Tests;

[TestClass]
public class AlarmRuleServiceTest
{
    [TestMethod]
    public async Task TestCreateAsync()
    {
        var options = new AlarmRuleUpsertModel();
        var requestUri = $"api/v1/AlarmRules";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PostAsync<AlarmRuleUpsertModel, Guid>(requestUri, options, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var messageTaskService = new Mock<AlarmRuleService>(caller.Object);
        var result = await messageTaskService.Object.CreateAsync(options);
        caller.Verify(provider => provider.PostAsync<AlarmRuleUpsertModel, Guid>(requestUri, options, default), Times.Once);
        Assert.AreNotEqual<Guid>(Guid.Empty, result);
    }

    [TestMethod]
    public async Task TestUpdateAsync()
    {
        Guid id = Guid.NewGuid();
        var options = new AlarmRuleUpsertModel();
        var requestUri = $"api/v1/AlarmRules/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, options, true, default)).Verifiable();
        var service = new Mock<AlarmRuleService>(caller.Object);
        await service.Object.UpdateAsync(id, options);
        caller.Verify(provider => provider.PutAsync(requestUri, options, true, default), Times.Once);
    }

    [TestMethod]
    public async Task TestDeleteAsync()
    {
        Guid id = Guid.NewGuid();
        var requestUri = $"api/v1/AlarmRules/{id}";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.DeleteAsync(requestUri, null, true, default)).Verifiable();
        var service = new Mock<AlarmRuleService>(caller.Object);
        await service.Object.DeleteAsync(id);
        caller.Verify(provider => provider.DeleteAsync(requestUri, null, true, default), Times.Once);
    }
}
