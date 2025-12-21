// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests;

[TestClass]
public class MasaDaprClientTest
{
    [TestMethod]
    public void TestSetAppId()
    {
        Mock<IServiceProvider> serviceProvider = new();
        string appId = "masa";
        var daprClient = new MasaDaprClient(serviceProvider.Object)
        {
            AppId = appId
        };
        Assert.AreEqual(appId, daprClient.AppId);

        Assert.ThrowsExactly<MasaArgumentException>(() => daprClient.AppId = null!);
        Assert.ThrowsExactly<MasaArgumentException>(() => daprClient.AppId = string.Empty);
        Assert.ThrowsExactly<MasaArgumentException>(() => daprClient.AppId = " ");
    }
}
