// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class AuthenticationServiceTest
{
    private const string DEFAULT_SCHEME = "Bearer";

    [TestMethod]
    public async Task TestExecuteAsync()
    {
        var tokenProvider = new TokenProvider()
        {
            AccessToken = "accessToken"
        };
        var authenticationService = new AuthenticationService(tokenProvider, null, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual(httpRequestMessage.Headers.Authorization.ToString(), $"{DEFAULT_SCHEME} {tokenProvider.AccessToken}");
    }

    [TestMethod]
    public async Task TestExecute2Async()
    {
        var tokenProvider = new TokenProvider()
        {
            AccessToken = ""
        };
        var authenticationService = new AuthenticationService(tokenProvider, null, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
    }
}
