// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Tests;

[TestClass]
public class McClientTest
{
    [TestMethod]
    public void TestAddMcClient()
    {
        var services = new ServiceCollection();
        services.AddMcClient("https://localhost:18102");
        var mcClient = services.BuildServiceProvider().GetRequiredService<IMcClient>();

        Assert.IsNotNull(mcClient);
    }

    [TestMethod]
    public void TestAddMcClientShouldThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => services.AddMcClient(mcServiceBaseAddress: null!));
    }

    [TestMethod]
    public void TestAddMcClientShouldThrowArgumentNullException2()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => services.AddMcClient(callerOptions: null!));
    }
}
