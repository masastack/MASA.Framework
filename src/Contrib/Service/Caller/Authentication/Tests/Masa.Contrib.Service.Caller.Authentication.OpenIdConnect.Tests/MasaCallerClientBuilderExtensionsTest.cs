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
        Assert.IsNotNull(options);

        Assert.AreEqual(0, options.Value.Options.Count);

        var middlewares = options.Value.Options.Where(o => o.Name.Equals(string.Empty)).Select(o => o.Middlewares).FirstOrDefault();
        Assert.IsNotNull(middlewares);

        Assert.AreEqual(0, middlewares.Count);

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

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ITokenValidatorHandler>());
    }
}
