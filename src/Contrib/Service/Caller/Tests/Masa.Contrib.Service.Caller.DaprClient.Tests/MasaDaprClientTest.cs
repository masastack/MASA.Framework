// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Tests;

[TestClass]
public class MasaDaprClientTest
{
    [TestMethod]
    public void TestSetAppId()
    {
        string appId = "masa";
        var daprClient = new MasaDaprClient(appId);
        Assert.AreEqual(appId, daprClient.AppId);

        Assert.ThrowsException<MasaArgumentException>(() => new MasaDaprClient(null!));
        Assert.ThrowsException<MasaArgumentException>(() => new MasaDaprClient(string.Empty));
        Assert.ThrowsException<MasaArgumentException>(() => new MasaDaprClient(" "));
    }
}
