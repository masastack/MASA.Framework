// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias Snowflake;
using System.Threading.Tasks;
using Snowflake.Microsoft.Extensions.DependencyInjection;
using DefaultWorkerProvider = Snowflake::Masa.Contrib.Data.IdGenerator.Snowflake.DefaultWorkerProvider;
using IWorkerProvider = Snowflake::Masa.Contrib.Data.IdGenerator.Snowflake.IWorkerProvider;
using SnowflakeGeneratorOptions = Snowflake::Masa.Contrib.Data.IdGenerator.Snowflake.SnowflakeGeneratorOptions;
using SnowflakeDependencyInjection = Snowflake.Microsoft.Extensions.DependencyInjection;

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

        Assert.IsTrue(ids.Distinct().Count() == ids.Count);
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
    public void TestCheckIdGeneratorOptions()
    {
        var services = new ServiceCollection();
        var generatorOptions = new SnowflakeGeneratorOptions(services);
        var workerProvider = new Mock<IWorkerProvider>();
        workerProvider.Setup(provider => provider.GetWorkerIdAsync()).ReturnsAsync(0);
        services.AddSingleton(workerProvider.Object);
        SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, generatorOptions);

        generatorOptions.BaseTime = DateTime.UtcNow.AddDays(1);
        var options = generatorOptions;
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, options));

        generatorOptions = new SnowflakeGeneratorOptions(services)
        {
            HeartbeatInterval = 5
        };
        var options1 = generatorOptions;
        SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, options1);

        workerProvider.Setup(provider => provider.GetWorkerIdAsync()).ReturnsAsync(200);
        generatorOptions = new SnowflakeGeneratorOptions(services)
        {
            WorkerIdBits = 2,
        };
        var generatorOptions1 = generatorOptions;
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
                => SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, generatorOptions1),
            $"workerId must be greater than 0 or less than or equal to {generatorOptions.MaxWorkerId}");

        generatorOptions = new SnowflakeGeneratorOptions(services)
        {
            TimestampType = TimestampType.Milliseconds,
            SequenceBits = 16,
            WorkerIdBits = 10,
        };

        Assert.ThrowsException<ArgumentOutOfRangeException>(()
                => SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, generatorOptions),
            $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than {22}");

        generatorOptions = new SnowflakeGeneratorOptions(services)
        {
            TimestampType = TimestampType.Seconds,
            SequenceBits = 16,
            WorkerIdBits = 16,
        };

        Assert.ThrowsException<ArgumentOutOfRangeException>(()
                => SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, generatorOptions),
            $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than {31}");

        generatorOptions = new SnowflakeGeneratorOptions(services)
        {
            HeartbeatInterval = 5
        };
        generatorOptions.EnableSupportDistributed();
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => SnowflakeDependencyInjection.ServiceCollectionExtensions.CheckIdGeneratorOptions(services, generatorOptions));
    }

    [TestMethod]
    public void TestNewId()
    {
        var services = new ServiceCollection();
        services.AddSnowflake();
        var snowflakeGenerator = MasaApp.GetService<ISnowflakeGenerator>();
        Assert.IsNotNull(snowflakeGenerator);
        Assert.IsTrue(snowflakeGenerator.NewId() > 0);
    }

    [TestMethod]
    public void TestNewIdByFactory()
    {
        var services = new ServiceCollection();
        services.AddSnowflake();
        MasaApp.Services = services;
        MasaApp.Build();
        var factory = MasaApp.GetService<IIdGeneratorFactory>();
        Assert.IsNotNull(factory);

        var snowflakeGenerator = factory.Create<long>();
        Assert.IsTrue(snowflakeGenerator.NewId() > 0L);
    }

    [TestMethod]
    public void TestSnowflakeGeneratorOptions()
    {
        var services = new ServiceCollection();
        var baseTime = DateTime.UtcNow.AddHours(-1);
        var snowflakeGeneratorOptions = new SnowflakeGeneratorOptions(services)
        {
            BaseTime = baseTime,
            MaxExpirationTime = 2,
            MaxCallBackTime = 5
        };
        Assert.AreEqual(2, snowflakeGeneratorOptions.MaxExpirationTime);
        Assert.AreEqual(5, snowflakeGeneratorOptions.MaxCallBackTime);
        Assert.AreEqual(baseTime, snowflakeGeneratorOptions.BaseTime);
    }

    [TestMethod]
    public void TestTimeCallBack()
    {
        var workerProvider = new Mock<IWorkerProvider>();
        var services = new ServiceCollection();
        SnowflakeGeneratorOptions snowflakeGeneratorOptions = new SnowflakeGeneratorOptions(services)
        {
            TimestampType = TimestampType.Milliseconds,
            MaxCallBackTime = 3000
        };
        var snowflakeIdGenerator = new CustomSnowflakeIdGenerator(workerProvider.Object, snowflakeGeneratorOptions);
        snowflakeIdGenerator.SetLastTimestamp(0);
        snowflakeIdGenerator.SetTilNextMillis(10);

        var result = snowflakeIdGenerator.TestTimeCallBack(1);
        Assert.AreEqual(true, result.Support);
        Assert.AreEqual(10, result.LastTimestamp);

        snowflakeIdGenerator.SetLastTimestamp(10000);
        result = snowflakeIdGenerator.TestTimeCallBack(10);
        Assert.AreEqual(false, result.Support);
        Assert.AreEqual(0, result.LastTimestamp);
    }

    [TestMethod]
    public async Task TestDefaultWorkerProvider()
    {
        Environment.SetEnvironmentVariable("WORKER_ID", "10");
        var workerProvider = new DefaultWorkerProvider();
        Assert.AreEqual(10, await workerProvider.GetWorkerIdAsync());
        Assert.AreEqual(Task.CompletedTask, workerProvider.RefreshAsync());
        Assert.AreEqual(Task.CompletedTask, workerProvider.LogOutAsync());
    }
}
