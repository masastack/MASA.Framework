// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class ThirdPartyIdpServiceTest
{
    [TestMethod]
    public async Task TestGetAllAsync()
    {
        var data = new List<ThirdPartyIdpModel>();
        var requestUri = $"api/thirdPartyIdp/getAll";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(requestUri, default)).ReturnsAsync(data).Verifiable();
        var multilevelCacheClient = new Mock<IMultilevelCacheClient>();
        var thirdPartyIdpService = new ThirdPartyIdpService(caller.Object, multilevelCacheClient.Object);
        var result = await thirdPartyIdpService.GetAllAsync();
        caller.Verify(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(requestUri, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetAllFromCacheAsync()
    {
        var data = new List<ThirdPartyIdpModel>();
        var requestUri = $"api/thirdPartyIdp/getAll";
        var caller = new Mock<ICaller>();
        var multilevelCacheClient = new Mock<IMultilevelCacheClient>();
        multilevelCacheClient.Setup(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(CacheKeyConsts.ALL_THIRD_PARTY_IDP, default)).ReturnsAsync(data).Verifiable();
        var thirdPartyIdpService = new ThirdPartyIdpService(caller.Object, multilevelCacheClient.Object);
        var result = await thirdPartyIdpService.GetAllFromCacheAsync();
        multilevelCacheClient.Verify(provider => provider.GetAsync<List<ThirdPartyIdpModel>>(CacheKeyConsts.ALL_THIRD_PARTY_IDP, default), Times.Once);
    }

    [TestMethod]
    public async Task TestGetLdapOptionsAsync()
    {
        var requestUri = $"api/thirdPartyIdp/ldapOptions";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<LdapOptionsModel>(requestUri, It.IsAny<object>(), default)).Verifiable();
        var multilevelCacheClient = new Mock<IMultilevelCacheClient>();
        var thirdPartyIdpService = new ThirdPartyIdpService(caller.Object, multilevelCacheClient.Object);
        var result = await thirdPartyIdpService.GetLdapOptionsAsync("ldap");
        caller.Verify(provider => provider.GetAsync<LdapOptionsModel>(requestUri, It.IsAny<object>(), default), Times.Once);
    }
}
