// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.BlazorServer.Tests;

[TestClass]
public class AuthenticationServiceTest
{
    [TestMethod]
    public async Task TestExecuteAsync()
    {
        var tokenProvider = new TokenProvider()
        {
            Authorization = "accessToken"
        };
        var authenticationService = new AuthenticationService(tokenProvider, AuthenticationConstant.DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual($"{AuthenticationConstant.DEFAULT_SCHEME} {tokenProvider.Authorization}",httpRequestMessage.Headers.Authorization.ToString());
    }

    [TestMethod]
    public async Task TestExecute2Async()
    {
        var tokenProvider = new TokenProvider()
        {
            Authorization = ""
        };
        var authenticationService = new AuthenticationService(tokenProvider, AuthenticationConstant.DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
    }
}
