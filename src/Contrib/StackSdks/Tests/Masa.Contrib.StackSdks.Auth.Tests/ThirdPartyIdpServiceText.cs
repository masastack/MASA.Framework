// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class ThirdPartyIdpServiceTest
{
    [TestMethod]
    public async Task TestGetAllThirdPartyIdpAsync()
    {
        var data = new List<ThirdPartyIdpModel>();
        var requestUri = $"api/thirdPartyIdp/getAll";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var thirdPartyIdpService = new ThirdPartyIdpService(caller.Object);
        var result = await thirdPartyIdpService.GetAllAsync();
        caller.Verify(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(requestUri, default), Times.Once);
    }
}
