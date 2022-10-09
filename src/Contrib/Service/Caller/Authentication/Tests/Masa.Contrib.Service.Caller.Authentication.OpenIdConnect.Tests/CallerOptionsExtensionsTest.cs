// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class CallerOptionsExtensionsTest
{
    [TestMethod]
    public void TestUseAuthentication()
    {
        CallerOptions callerOptions = new CallerOptions(new ServiceCollection());
        callerOptions.UseAuthentication();

        var serviceProvider = callerOptions.Services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<TokenProvider>());
        Assert.IsNotNull(serviceProvider.GetService<IAuthenticationService>());
        Assert.IsNull(serviceProvider.GetService<ITokenValidatorHandler>());
    }
}
