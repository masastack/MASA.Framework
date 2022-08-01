// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class AuthClientTest
{
    [TestMethod]
    public void TestAddAuthClient()
    {
        var services = new ServiceCollection();
        services.AddMasaIdentityModel(IdentityType.MultiEnvironment);
        services.AddAuthClient("https://localhost:18102");
        var authClient = services.BuildServiceProvider().GetRequiredService<IAuthClient>();

        Assert.IsNotNull(authClient);
    }

    [TestMethod]
    public void TestAddAuthClientNoAddMasaIdentity()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<Exception>(() => services.AddAuthClient("https://localhost:18102"),
            "Please add IMultiEnvironmentUserContext first.");
    }

    [TestMethod]
    public void TestAddAuthClientShouldThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => services.AddAuthClient(authServiceBaseAddress: null!));
    }

    [TestMethod]
    public void TestAddAuthClientShouldThrowArgumentNullException2()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentNullException>(() => services.AddAuthClient(callerOptions: null!));
    }
}

