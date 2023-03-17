// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Alert.Tests;

[TestClass]
public class AlertClientTest
{
    [TestMethod]
    public void TestAddAlertClient()
    {
        var services = new ServiceCollection();
        services.AddAlertClient("https://localhost:19701");
        var mcClient = services.BuildServiceProvider().GetRequiredService<IAlertClient>();

        Assert.IsNotNull(mcClient);
    }

    [TestMethod]
    public void TestAddAlertClientShouldThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<MasaArgumentException>(() => services.AddAlertClient(alertServiceBaseAddress: null!));
    }

    [TestMethod]
    public void TestAddAlertClientShouldThrowArgumentNullException2()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<MasaArgumentException>(() => services.AddAlertClient(callerOptionsBuilder: null!));
    }
}
