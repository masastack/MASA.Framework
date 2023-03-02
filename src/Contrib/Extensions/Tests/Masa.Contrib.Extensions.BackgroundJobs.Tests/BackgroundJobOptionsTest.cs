// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Tests;

[TestClass]
public class BackgroundJobOptionsTest
{
    [TestMethod]
    public void TestBackgroundJobOptions()
    {
        var backgroundJobOptions = new BackgroundJobOptions();
        Assert.AreEqual(30, backgroundJobOptions.BatchSize);
        Assert.AreEqual(30, backgroundJobOptions.MaxRetryTimes);
        Assert.AreEqual(60, backgroundJobOptions.FirstWaitDuration);
        Assert.AreEqual(2, backgroundJobOptions.WaitDuration);

        backgroundJobOptions.BatchSize = 1;
        Assert.AreEqual(1, backgroundJobOptions.BatchSize);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.BatchSize = 0);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.BatchSize = -1);

        backgroundJobOptions.MaxRetryTimes = 0;
        Assert.AreEqual(0, backgroundJobOptions.MaxRetryTimes);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.MaxRetryTimes = -1);

        backgroundJobOptions.FirstWaitDuration = 1;
        Assert.AreEqual(1, backgroundJobOptions.FirstWaitDuration);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.FirstWaitDuration = 0);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.FirstWaitDuration = -1);

        backgroundJobOptions.WaitDuration = 1;
        Assert.AreEqual(1, backgroundJobOptions.WaitDuration);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.WaitDuration = 0);
        Assert.ThrowsException<MasaArgumentException>(() => backgroundJobOptions.WaitDuration = -1);
    }
}
