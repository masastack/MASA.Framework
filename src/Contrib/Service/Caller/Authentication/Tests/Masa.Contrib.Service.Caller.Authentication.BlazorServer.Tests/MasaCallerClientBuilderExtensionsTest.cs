// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.BlazorServer.Tests;

[TestClass]
public class MasaCallerClientBuilderExtensionsTest
{
    [TestMethod]
    public async Task TestUseAuthenticationAsync()
    {
        var services = new ServiceCollection();
        Mock<IMasaCallerClientBuilder> masaCallerClientBuilder = new();
        masaCallerClientBuilder.Setup(builder => builder.Services).Returns(services);
        masaCallerClientBuilder.Setup(builder => builder.Name).Returns(string.Empty);
        masaCallerClientBuilder.Object.UseAuthentication();
        var serviceProvider = services.BuildServiceProvider();
        var factoryOptions = serviceProvider.GetService<IOptions<AuthenticationServiceFactoryOptions>>();
        Assert.IsNotNull(factoryOptions);
        Assert.AreEqual(ServiceLifetime.Transient, factoryOptions.Value.Lifetime);
        Assert.AreEqual(1, factoryOptions.Value.Options.Count);
        Assert.AreEqual(string.Empty, factoryOptions.Value.Options[0].Name);
        var authenticationService = serviceProvider.GetService<IAuthenticationService>();
        Assert.IsNotNull(authenticationService);
        var httpRequestMessage = new HttpRequestMessage();
        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNull(httpRequestMessage.Headers.Authorization);

        var tokenProvider = serviceProvider.GetService<TokenProvider>();
        Assert.IsNotNull(tokenProvider);
        tokenProvider.Authorization = "masa";

        await authenticationService.ExecuteAsync(httpRequestMessage);
        Assert.IsNotNull(httpRequestMessage.Headers.Authorization);
        Assert.AreEqual($"{AuthenticationConstant.DEFAULT_SCHEME} {tokenProvider.Authorization}",
            httpRequestMessage.Headers.Authorization.ToString());
    }
}
