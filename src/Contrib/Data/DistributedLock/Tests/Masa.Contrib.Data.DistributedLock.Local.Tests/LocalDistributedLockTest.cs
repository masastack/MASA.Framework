﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLock.Local.Tests;

[TestClass]
public class LocalDistributedLockTest
{
    private IDistributedLock DistributedLock { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        var services = new ServiceCollection();
        services.AddLocalDistributedLock();
        DistributedLock = services.GetInstance<IDistributedLock>();
    }

    [TestMethod]
    public void TestDistributedLock()
    {
        using var obj = DistributedLock.TryGet("test", TimeSpan.FromSeconds(1));
        Assert.IsNotNull(obj);
    }

    [TestMethod]
    public async Task TestDistributedLockAsync()
    {
        await using var obj = await DistributedLock.TryGetAsync("test", TimeSpan.FromSeconds(1));
        Assert.IsNotNull(obj);
    }
}
