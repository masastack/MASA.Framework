// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class AuthenticationServiceTest
{
    private const string DEFAULT_SCHEME = "Bearer";
    private IServiceCollection _services;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.AddScoped<TokenProvider>();
    }

    [TestMethod]
    public async Task TestAuthenticationMiddlewareAsync()
    {
        var tokenProvider = new TokenProvider()
        {
            AccessToken = "accessToken"
        };
        _services.AddScoped(_ => tokenProvider);
        _httpContextAccessor.Setup(accessor => accessor.HttpContext)?.Returns(new DefaultHttpContext()
        {
            RequestServices = _services.BuildServiceProvider()
        });
        Mock<IResponseMessage> responseMessage = new();
        var authenticationService = new AuthenticationMiddleware(_httpContextAccessor.Object, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        var masaHttpContext = new MasaHttpContext(responseMessage.Object, httpRequestMessage);

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
        _services.AddScoped<TokenProvider>();
        _httpContextAccessor.Setup(accessor => accessor.HttpContext)?.Returns(new DefaultHttpContext()
        {
            RequestServices = _services.BuildServiceProvider()
        });
        Mock<IResponseMessage> responseMessage = new();
        var authenticationService = new AuthenticationMiddleware(_httpContextAccessor.Object, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        var masaHttpContext = new MasaHttpContext(responseMessage.Object, httpRequestMessage);

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

    [TestMethod]
    public async Task TestAuthenticationMiddleware3Async()
    {
        Mock<ITokenValidatorHandler> tokenValidatorHandler = new();
        _services.AddSingleton(_ => tokenValidatorHandler.Object);
        _services.AddScoped<TokenProvider>();
        _httpContextAccessor.Setup(accessor => accessor.HttpContext)?.Returns(new DefaultHttpContext()
        {
            RequestServices = _services.BuildServiceProvider()
        });
        Mock<IResponseMessage> responseMessage = new();
        var authenticationService = new AuthenticationMiddleware(_httpContextAccessor.Object, DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers = { }
        };
        var masaHttpContext = new MasaHttpContext(responseMessage.Object, httpRequestMessage);

        var times = 0;
        CallerHandlerDelegate callerHandlerDelegate = () =>
        {
            times++;
            return Task.FromResult(times);
        };

        await authenticationService.HandleAsync(masaHttpContext, callerHandlerDelegate);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual(1, times);

        tokenValidatorHandler.Verify(tokenValidator => tokenValidator.ValidateTokenAsync(It.IsAny<TokenProvider>()), Times.Once);
    }
}
