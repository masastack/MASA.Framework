// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class DispatcherOptionTest
{
    private IntegrationEventOptions _options;

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        _options = new IntegrationEventOptions(services, AppDomain.CurrentDomain.GetAssemblies());
    }

    [TestMethod]
    public void TestSetLocalRetryTimes()
    {
        Assert.IsTrue(_options.LocalRetryTimes == 3);
        _options.LocalRetryTimes = 5;
        Assert.IsTrue(_options.LocalRetryTimes == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.LocalRetryTimes = 0);
    }

    [TestMethod]
    public void TestSetMaxRetryTimes()
    {
        Assert.IsTrue(_options.MaxRetryTimes == 10);
        _options.MaxRetryTimes = 5;
        Assert.IsTrue(_options.MaxRetryTimes == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.MaxRetryTimes = 0);
    }

    [TestMethod]
    public void TestSetFailedRetryInterval()
    {
        Assert.IsTrue(_options.FailedRetryInterval == 60);
        _options.FailedRetryInterval = 5;
        Assert.IsTrue(_options.FailedRetryInterval == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.FailedRetryInterval = 0);
    }

    [TestMethod]
    public void TestSetMinimumRetryInterval()
    {
        Assert.IsTrue(_options.MinimumRetryInterval == 60);
        _options.MinimumRetryInterval = 5;
        Assert.IsTrue(_options.MinimumRetryInterval == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.MinimumRetryInterval = 0);
    }

    [TestMethod]
    public void TestSetLocalFailedRetryInterval()
    {
        Assert.IsTrue(_options.LocalFailedRetryInterval == 3);
        _options.LocalFailedRetryInterval = 5;
        Assert.IsTrue(_options.LocalFailedRetryInterval == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.LocalFailedRetryInterval = -1);
    }

    [TestMethod]
    public void TestSetRetryBatchSize()
    {
        Assert.IsTrue(_options.RetryBatchSize == 100);
        _options.RetryBatchSize = 5;
        Assert.IsTrue(_options.RetryBatchSize == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.RetryBatchSize = -1);
    }

    [TestMethod]
    public void TestSetCleaningLocalQueueExpireInterval()
    {
        Assert.IsTrue(_options.CleaningLocalQueueExpireInterval == 60);
        _options.CleaningLocalQueueExpireInterval = 5;
        Assert.IsTrue(_options.CleaningLocalQueueExpireInterval == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.CleaningLocalQueueExpireInterval = 0);
    }

    [TestMethod]
    public void TestSetCleaningExpireInterval()
    {
        Assert.IsTrue(_options.CleaningExpireInterval == 300);
        _options.CleaningExpireInterval = 5;
        Assert.IsTrue(_options.CleaningExpireInterval == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.CleaningExpireInterval = 0);
    }

    [TestMethod]
    public void TestSetPublishedExpireTime()
    {
        Assert.IsTrue(_options.PublishedExpireTime == 24 * 3600);
        _options.PublishedExpireTime = 24 * 3 * 3600;
        Assert.IsTrue(_options.PublishedExpireTime == 24 * 3 * 3600);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.PublishedExpireTime = 0);
    }

    [TestMethod]
    public void TestSetDeleteBatchCount()
    {
        Assert.IsTrue(_options.DeleteBatchCount == 1000);
        _options.DeleteBatchCount = 100;
        Assert.IsTrue(_options.DeleteBatchCount == 100);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.DeleteBatchCount = 0);
    }

    [TestMethod]
    public void TestGetCurrentTime()
    {
        Assert.IsTrue(_options.GetCurrentTime == null);
        _options.GetCurrentTime = () => DateTime.UtcNow;
        Assert.IsTrue((_options.GetCurrentTime.Invoke() - DateTime.UtcNow).Minutes == 0);
    }

    [TestMethod]
    public void SetBatchSize()
    {
        Assert.IsTrue(_options.BatchSize == 20);
        _options.BatchSize = 5;
        Assert.IsTrue(_options.BatchSize == 5);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.BatchSize = -1);
    }

    [TestMethod]
    public void SetExecuteInterval()
    {
        Assert.AreEqual(1,_options.ExecuteInterval);
        _options.ExecuteInterval = 2;
        Assert.AreEqual(2, _options.ExecuteInterval);

        Assert.ThrowsExactly<MasaArgumentException>(() => _options.ExecuteInterval = 0);
    }
}
