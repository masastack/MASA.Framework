// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Tests;

[TestClass]
public class SsoClientTest
{
    [TestMethod]
    public void TestAddSsoClient()
    {
        var services = new ServiceCollection();
        services.AddSsoClient(() => "https://localhost:18102");
        var ssoClient = services.BuildServiceProvider().GetRequiredService<ISsoClient>();

        Assert.IsNotNull(ssoClient);
    }
}

