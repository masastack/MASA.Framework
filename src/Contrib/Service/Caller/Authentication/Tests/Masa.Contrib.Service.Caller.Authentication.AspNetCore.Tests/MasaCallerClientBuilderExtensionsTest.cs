// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.AspNetCore.Tests;

[TestClass]
public class MasaCallerClientBuilderExtensionsTest
{
    [TestMethod]
    public async Task TestUseAuthenticationAndTokenIsNullAsync()
    {
        var services = new ServiceCollection();
        var httpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers =
                {
                }
            }
        };
        services.AddSingleton<IHttpContextAccessor>(_ => new HttpContextAccessor()
        {
            HttpContext = httpContext
        });

        Mock<IMasaCallerClientBuilder> masaCallerClientBuilder = new();
        masaCallerClientBuilder.Setup(builder => builder.Services).Returns(services);
        masaCallerClientBuilder.Setup(builder => builder.Name).Returns(string.Empty);
        masaCallerClientBuilder.Object.UseAuthentication();
        var serviceProvider = services.BuildServiceProvider();

        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        Assert.IsNotNull(authenticationService);
        var httpRequestMessage = new HttpRequestMessage();
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);
    }

    [TestMethod]
    public async Task TestUseAuthenticationAsync()
    {
        var services = new ServiceCollection();
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
        services.AddSingleton<IHttpContextAccessor>(_ => new HttpContextAccessor()
        {
            HttpContext = httpContext
        });

        Mock<IMasaCallerClientBuilder> masaCallerClientBuilder = new();
        masaCallerClientBuilder.Setup(builder => builder.Services).Returns(services);
        masaCallerClientBuilder.Setup(builder => builder.Name).Returns(string.Empty);
        masaCallerClientBuilder.Object.UseAuthentication();
        var serviceProvider = services.BuildServiceProvider();

        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        Assert.IsNotNull(authenticationService);
        var httpRequestMessage = new HttpRequestMessage();
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);

        Assert.AreEqual($"{AuthenticationConstant.DEFAULT_SCHEME} {token}",
            httpRequestMessage.Headers.Authorization.ToString());
    }
}
