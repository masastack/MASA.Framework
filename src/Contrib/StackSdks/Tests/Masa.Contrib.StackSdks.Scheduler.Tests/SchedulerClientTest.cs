// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler.Tests;

[TestClass]
public class SchedulerClientTest
{
    [TestMethod]
    public void TestAddSchedulerClientByOptions()
    {
        var services = new ServiceCollection();

        services.AddSchedulerClient(option =>
        {
            option.UseHttpClient(builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
            });
        });

        var schedulerClient = services.BuildServiceProvider().GetService<ISchedulerClient>();
        Assert.IsNotNull(schedulerClient);
    }

    [TestMethod]
    public void TestAddSchedulerClient()
    {
        var services = new ServiceCollection();
        services.AddSchedulerClient("https://github.com");
        var schedulerClient = services.BuildServiceProvider().GetService<ISchedulerClient>();
        Assert.IsNotNull(schedulerClient);
    }

    [TestMethod]
    public void TestAddSchedulerClientShouldThrowArgumentNullException()
    {
        var services = new ServiceCollection();

        Assert.ThrowsException<ArgumentNullException>(() => services.AddSchedulerClient(""));
    }

    [TestMethod]
    public void TestAddSchedulerClientShouldThrowArgumentNullException2()
    {
        var services = new ServiceCollection();

        Assert.ThrowsException<ArgumentNullException>(() => services.AddSchedulerClient(callerOptionsBuilder: null!));
    }

}
