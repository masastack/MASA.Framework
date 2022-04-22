using Masa.BuildingBlocks.BasicAbility.Pm;

namespace Masa.Contrib.BasicAbility.Pm.Tests;

[TestClass]
public class PmClientTest
{
    [TestMethod]
    public void TestAddPmClient()
    {
        var services = new ServiceCollection();

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }

    [TestMethod]
    public void TestAddPmClient1()
    {
        var services = new ServiceCollection();

        services.AddPmClient("https://github.com");

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }

    [TestMethod]
    public void TestAddPmClientShouldThrowArgumentNullException()
    {
        var services = new ServiceCollection();

        Assert.ThrowsException<ArgumentNullException>(() => services.AddPmClient(""));
    }

    [TestMethod]
    public void TestAddMultiplePmClient()
    {
        var services = new ServiceCollection();

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        services.AddPmClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Name = "masa.contrib.basicability.pm";
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }
}
