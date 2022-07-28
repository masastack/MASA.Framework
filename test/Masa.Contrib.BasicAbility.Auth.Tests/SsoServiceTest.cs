// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class SsoServiceTest
{
    [TestMethod]
    public async Task GetSecurityTokenAsync()
    {
        var data = new SecurityTokenModel("region", "accessKeyId", "accessKeySecret", "stsToken", "bucket");
        var requestUri = $"api/oss/GetSecurityToken";
        var callerProvider = new Mock<ICallerProvider>();
        callerProvider.Setup(provider => provider.GetAsync<SecurityTokenModel>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var ossService = new OssService(callerProvider.Object);
        var result = await ossService.GetSecurityTokenAsync();
        callerProvider.Verify(provider => provider.GetAsync<SecurityTokenModel>(requestUri, default), Times.Once);
        Assert.IsTrue(result is not null);
    }
}
