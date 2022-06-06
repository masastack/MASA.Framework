namespace Masa.Contrib.BasicAbility.Auth.Tests;

[TestClass]
public class AuthClientTest
{
    [TestMethod]
    public void TestAddAuthClient()
    {
        var services = new ServiceCollection();
        services.AddAuthClient("https://localhost:18102");
        var authClient = services.BuildServiceProvider().GetRequiredService<IAuthClient>();

        Assert.IsNotNull(authClient);
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

