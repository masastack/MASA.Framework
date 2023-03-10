// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.AspNetCore.Tests;

[TestClass]
public class AuthenticationServiceTest
{
    [TestMethod]
    public async Task TestExecuteAndTokenIsNullAsync()
    {
        var httpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers =
                {
                }
            }
        };
        var httpContextAccessor = new HttpContextAccessor()
        {
            HttpContext = httpContext
        };
        var authenticationService = new AuthenticationService(httpContextAccessor, AuthenticationConstant.DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers =
            {
            }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
    }

    [TestMethod]
    public async Task TestExecuteAsync()
    {
        var token = "token";
        var httpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers =
                {
                    {
                        "Authorization", $"{AuthenticationConstant.DEFAULT_SCHEME} {token}"
                    }
                }
            }
        };
        var httpContextAccessor = new HttpContextAccessor()
        {
            HttpContext = httpContext
        };
        var authenticationService = new AuthenticationService(httpContextAccessor, AuthenticationConstant.DEFAULT_SCHEME);
        var httpRequestMessage = new HttpRequestMessage()
        {
            Headers =
            {
            }
        };
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual($"{AuthenticationConstant.DEFAULT_SCHEME} {token}", httpRequestMessage.Headers.Authorization.ToString());
    }
}
