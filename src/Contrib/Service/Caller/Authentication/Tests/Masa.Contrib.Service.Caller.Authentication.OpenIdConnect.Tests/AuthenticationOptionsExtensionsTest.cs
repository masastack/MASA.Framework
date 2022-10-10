// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Tests;

[TestClass]
public class AuthenticationOptionsExtensionsTest
{
    [TestMethod]
    public void TestUseJwtBearer()
    {
        var services = new ServiceCollection();
        CallerOptions callerOptions = new CallerOptions(services);
        callerOptions.UseAuthentication(options => options.UseJwtBearer(options =>
        {

        }, clientRefreshTokenOptions =>
        {

        }));

        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<ITokenValidatorHandler>());
    }
}
