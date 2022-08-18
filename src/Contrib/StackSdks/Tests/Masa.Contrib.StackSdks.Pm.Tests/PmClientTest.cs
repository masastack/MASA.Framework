// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Pm.Tests;

[TestClass]
public class PmClientTest
{
    public const string PM_CALLER_NAME = "masa.contrib.basicability.pm";

    [TestMethod]
    public void TestAddPmClient()
    {
        var services = new ServiceCollection();

        services.AddPmClient(option =>
        {
            option.UseHttpClient(PM_CALLER_NAME, builder =>
            {
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
            option.UseHttpClient(PM_CALLER_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        services.AddPmClient(option =>
        {
            option.UseHttpClient(PM_CALLER_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var pmClient = services.BuildServiceProvider().GetRequiredService<IPmClient>();
        Assert.IsNotNull(pmClient);
    }
}
