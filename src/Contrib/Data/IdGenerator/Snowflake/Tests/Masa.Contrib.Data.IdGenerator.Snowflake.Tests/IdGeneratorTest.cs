// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests;

[TestClass]
public class IdGeneratorTest
{
    [TestMethod]
    public void TestEnableMachineClock()
    {
        var services = new ServiceCollection();
        services.AddSnowflake(opt =>
        {
            opt.EnableMachineClock = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<long>>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.NewId();
            ids.Add(id);
            count++;
        }

        if (ids.Distinct().Count() != ids.Count)
            throw new Exception("duplicate id");
    }

    [TestMethod]
    public void TestDisableMachineClock()
    {
        var services = new ServiceCollection();
        services.AddSnowflake();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<long>>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.NewId();
            ids.Add(id);
            count++;
        }

        if (ids.Distinct().Count() != ids.Count)
            throw new Exception("duplicate id");
    }

    [TestMethod]
    public void TestErrorBaseTimeReturnThrowArgumentOutOfRangeException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            services.AddSnowflake(options =>
            {
                options.BaseTime = DateTime.UtcNow.AddHours(1);
            });
        });
    }

    [TestMethod]
    public void TestErrorWorkerIdReturnThrowArgumentOutOfRangeException()
    {
        long maxWorkerId = ~(-1L << 5);
        long workerId = maxWorkerId + 1;
        Environment.SetEnvironmentVariable("WORKER_ID", workerId.ToString());
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            services.AddSnowflake(options =>
            {
                options.WorkerIdBits = 5;
            });
        });
    }

    [TestMethod]
    public void TestErrorSequenceBitsReturnThrowArgumentOutOfRangeException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            services.AddSnowflake(options =>
            {
                options.SequenceBits = 21;
                options.WorkerIdBits = 2;
            });
        });
    }

    [TestMethod]
    public void TestSnowflakeGuidReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        services.AddSnowflake();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetService<IIdGenerator<long>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(SnowflakeIdGenerator));

        Assert.IsNotNull(serviceProvider.GetService<IIdGenerator>());
        Assert.IsNull(serviceProvider.GetService<IIdGenerator<Guid>>());
        Assert.IsNotNull(serviceProvider.GetService<ISnowflakeGenerator>());
    }

    [TestMethod]
    public void TestSnowflakeGuidByMasaAppReturnIdGeneratorIsNotNull()
    {
        var services = new ServiceCollection();
        MasaApp.Services = services;
        services.AddSnowflake();

        var idGenerator = MasaApp.GetService<IIdGenerator<long>>();
        Assert.IsNotNull(idGenerator);
        Assert.IsTrue(idGenerator.GetType() == typeof(SnowflakeIdGenerator));

        Assert.IsNotNull(MasaApp.GetService<IIdGenerator>());
        Assert.IsNull(MasaApp.GetService<IIdGenerator<Guid>>());
        Assert.IsNotNull(MasaApp.GetService<ISnowflakeGenerator>());
    }
}
