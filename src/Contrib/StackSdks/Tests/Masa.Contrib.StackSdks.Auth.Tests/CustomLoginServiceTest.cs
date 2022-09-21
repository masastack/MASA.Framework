// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class CustomLoginServiceTest
{
    [TestMethod]
    public async Task TestGetCustomLoginByClientIdAsync()
    {
        var data = new CustomLoginModel();
        string clientId = Guid.NewGuid().ToString();
        var requestUri = $"api/sso/customLogin/getByClientId";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<object, CustomLoginModel>(requestUri, It.IsAny<object>(), default)).ReturnsAsync(data).Verifiable();
        var customLoginService = new CustomLoginService(caller.Object);
        var result = await customLoginService.GetCustomLoginByClientIdAsync(clientId);
        caller.Verify(provider => provider.GetAsync<object, CustomLoginModel>(requestUri, It.IsAny<object>(), default), Times.Once);
    }
}
