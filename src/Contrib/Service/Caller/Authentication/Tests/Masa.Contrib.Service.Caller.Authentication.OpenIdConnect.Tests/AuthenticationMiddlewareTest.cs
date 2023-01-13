// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class AuthenticationServiceTest
{
    private const string DEFAULT_SCHEME = "Bearer";

    [TestMethod]
    public async Task TestAuthenticationMiddlewareAsync()
    {
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IResponseMessage> responseMessage = new();
        var tokenProvider = new TokenProvider()
        {
            AccessToken = "accessToken"
        };
        var authenticationService = new AuthenticationMiddleware(tokenProvider, null, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        var masaHttpContext = new MasaHttpContext(serviceProvider.Object, responseMessage.Object, httpRequestMessage);

        var times = 0;
        CallerHandlerDelegate callerHandlerDelegate = () =>
        {
            times++;
            return Task.FromResult(times);
        };

        await authenticationService.HandleAsync(masaHttpContext, callerHandlerDelegate);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual(httpRequestMessage.Headers.Authorization.ToString(), $"{DEFAULT_SCHEME} {tokenProvider.AccessToken}");
        Assert.AreEqual(1, times);
    }

    [TestMethod]
    public async Task TestAuthenticationMiddleware2Async()
    {
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IResponseMessage> responseMessage = new();
        var tokenProvider = new TokenProvider();
        var authenticationService = new AuthenticationMiddleware(tokenProvider, null, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        var masaHttpContext = new MasaHttpContext(serviceProvider.Object, responseMessage.Object, httpRequestMessage);

        var times = 0;
        CallerHandlerDelegate callerHandlerDelegate = () =>
        {
            times++;
            return Task.FromResult(times);
        };

        await authenticationService.HandleAsync(masaHttpContext, callerHandlerDelegate);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual(1, times);
    }
}
