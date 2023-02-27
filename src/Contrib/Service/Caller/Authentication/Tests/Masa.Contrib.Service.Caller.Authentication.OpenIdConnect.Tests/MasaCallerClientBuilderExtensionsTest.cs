// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class MasaCallerClientBuilderExtensionsTest
{
    [TestMethod]
    public void TestUseAuthentication()
    {
        var services = new ServiceCollection();
        Mock<IMasaCallerClientBuilder> masaCallerClient = new();
        masaCallerClient.Setup(client => client.Name).Returns(string.Empty);
        masaCallerClient.Setup(client => client.Services).Returns(services);
        masaCallerClient.Object.UseAuthentication();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<CallerMiddlewareFactoryOptions>>();
        Assert.IsNull(options);

        Assert.IsNull(serviceProvider.GetService<ITokenValidatorHandler>());
    }

    [TestMethod]
    public void TestUseJwt()
    {
        var services = new ServiceCollection();
        Mock<IMasaCallerClientBuilder> masaCallerClient = new();
        masaCallerClient.Setup(client => client.Name).Returns(string.Empty);
        masaCallerClient.Setup(client => client.Services).Returns(services);
        masaCallerClient.Object.UseAuthentication(options =>
        {
            options.UseJwtBearer(jwtTokenValidatorOptions =>
            {

            }, clientRefreshTokenOptions =>
            {

            });
        });
        var accessToken = "token";
        var httpContext = new DefaultHttpContext()
        {
            Request =
            {
                Headers =
                {
                    {
                        "Authorization", $"Bearer {accessToken}"
                    }
                }
            }
        };
        services.AddSingleton<IHttpContextAccessor>(_ => new HttpContextAccessor()
        {
            HttpContext = httpContext
        });

        var serviceProvider = services.BuildServiceProvider();
        var tokenValidatorHandler = serviceProvider.GetService<ITokenValidatorHandler>();
        Assert.IsNotNull(tokenValidatorHandler);
        Assert.IsNotNull(serviceProvider.GetService<IAuthenticationService>());
        var tokenProvider = serviceProvider.GetService<TokenProvider>();
        Assert.IsNotNull(tokenProvider);
        Assert.AreEqual(accessToken, tokenProvider.AccessToken);
    }
}
