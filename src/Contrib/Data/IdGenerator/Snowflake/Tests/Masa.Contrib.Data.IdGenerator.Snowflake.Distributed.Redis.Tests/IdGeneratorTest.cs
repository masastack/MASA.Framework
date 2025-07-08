// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias SnowflakeRedis;
using SnowflakeRedis::Masa.Contrib.Data.IdGenerator.Snowflake;

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.Tests;

#pragma warning disable CS0618
[TestClass]
public class IdGeneratorTest
{
    private const string REDIS_HOST = "localhost";
    private IOptions<RedisConfigurationOptions> _redisOptions;
    private IDatabase _database;
    private string _currentWorkerKey;
    private string _inUseWorkerKey;
    private string _logOutWorkerKey;
    private string _getWorkerIdKey;

    [TestInitialize]
    public async Task InitRedisDataAsync()
    {
        RedisConfigurationOptions redisConfigurationOptions = new()
        {
            Password = "",
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            }
        };
        _redisOptions = Options.Create(redisConfigurationOptions);
        var options = (ConfigurationOptions)_redisOptions.Value;
        var connection = await ConnectionMultiplexer.ConnectAsync(options);
        _database = connection.GetDatabase(options.DefaultDatabase ?? 0);
        _currentWorkerKey = "Int64.snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";

        _database.KeyDelete(_currentWorkerKey);
        _database.KeyDelete(_inUseWorkerKey);
        _database.KeyDelete(_logOutWorkerKey);
        _database.KeyDelete(_getWorkerIdKey);
    }

    [TestMethod]
    public void TestErrorHeartbeatIntervalReturnThrowArgumentOutOfRangeException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            services.AddSnowflake(options =>
            {
                options.UseRedis();
                options.HeartbeatInterval = 99;
            });
        });
    }

    [TestMethod]
    public void TestUseRedisAndNotUseRedisAndSpecialRedisConfigurationOptions2()
    {
        var services = new ServiceCollection();
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Password = "",
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            }
        };
        services.AddDistributedCache(builder => builder.UseStackExchangeRedisCache(redisConfigurationOptions));
        var snowflakeGeneratorOptions = Substitute.For<SnowflakeGeneratorOptions>(services);
        snowflakeGeneratorOptions.HeartbeatInterval = 30 * 1000;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => snowflakeGeneratorOptions.UseRedis(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.IdleTimeOut = 10 * 1000;
        }, redisConfigurationOptions));
    }

    [TestMethod]
    public void TestUseRedisAndNotUseRedisAndSpecialRedisConfigurationOptions3()
    {
        var services = new ServiceCollection();
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Password = "",
            DefaultDatabase = 5,
            Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            }
        };
        services.AddDistributedCache(builder => builder.UseStackExchangeRedisCache(redisConfigurationOptions));
        var snowflakeGeneratorOptions = Substitute.For<SnowflakeGeneratorOptions>(services);
        snowflakeGeneratorOptions.EnableMachineClock = true;
        snowflakeGeneratorOptions.UseRedis(null, redisConfigurationOptions);

        var distributedCacheClient = Substitute.For<IDistributedCacheClient>();
        services.AddSingleton(distributedCacheClient);

        var serviceProvider = services.BuildServiceProvider();
        var snowflakeGenerator = serviceProvider.GetService<ISnowflakeGenerator>();
        Assert.IsNotNull(snowflakeGenerator);
    }

    [TestMethod]
    public void TestDistributedSnowflake()
    {
        var services = new ServiceCollection();
        services.AddSnowflake(option => option.UseRedis(distribute =>
        {
            distribute.GetWorkerIdMinInterval = 0;
        }, options =>
        {
            options.Password = "";
            options.DefaultDatabase = 2;

            options.Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            };
        }));
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

        Assert.AreEqual(ids.Count, ids.Distinct().Count());
    }

    [TestMethod]
    public async Task TestDistributedWorkerAsync()
    {
        var services = new ServiceCollection();
        services.AddSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.UseRedis(
                option => option.GetWorkerIdMinInterval = 0,
                redisOptions =>
                {
                    redisOptions.Password = "";
                    redisOptions.DefaultDatabase = 2;
                    redisOptions.Servers = new List<RedisServerOptions>()
                    {
                        new(REDIS_HOST)
                    };
                });
        });

        var serviceProvider = services.BuildServiceProvider();
        var workerIdProvider = serviceProvider.GetRequiredService<IWorkerProvider>();
        List<long> workerIds = new();
        var maxWorkerId = ~(-1L << 2);
        for (int index = 0; index <= maxWorkerId; index++)
        {
            var workerId = await workerIdProvider.GetWorkerIdAsync();
            await workerIdProvider.LogOutAsync();
            workerIds.Add(workerId);
        }

        Assert.IsTrue(workerIds.Distinct().Count() == workerIds.Count && workerIds.Count == maxWorkerId + 1);
    }

    [TestMethod]
    public void TestDistributedWorkerAndEnableMachineClock()
    {
        var services = new ServiceCollection();
        services.AddDistributedCache(builder => builder.UseStackExchangeRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new(REDIS_HOST)
            };
        }));

        services.AddSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.UseRedis(option =>
            {
                option.GetWorkerIdMinInterval = 0;
            });
            distributedIdGeneratorOptions.EnableMachineClock = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<long>>();
        var id = idGenerator.NewId();
        var maxSequenceBit = ~(-1L << 2);
        for (int i = 1; i < maxSequenceBit; i++)
        {
            var idTemp = idGenerator.NewId();
            Assert.IsTrue(i + id == idTemp);
        }
    }

    [TestMethod]
    public async Task TestGetWorkerIdAsync()
    {
        var services = new ServiceCollection();

        var workerIdBits = 2;
        var workerIdProvider = GetWorkerProvider(services, workerIdBits);

        List<long> workerIds = new();
        var errCount = 0;
        var maxWorkerId = ~(-1L << workerIdBits);

        for (int index = 0; index <= maxWorkerId + 1; index++)
        {
            try
            {
                var workerId = await workerIdProvider.GetWorkerIdAsync();
                await workerIdProvider.LogOutAsync();
                workerIds.Add(workerId);
            }
            catch (MasaException ex)
            {
                errCount = 1;
                Assert.IsTrue(ex.Message == "No WorkerId available" && index == maxWorkerId + 1);
            }
        }
        Assert.IsTrue(workerIds.Count == maxWorkerId + 1);
        Assert.IsTrue(errCount == 1);
    }

    [TestMethod]
    public async Task TestGetDistributedLockFailedAsync()
    {
        var workerIdBits = 2;
        var maxWorkerId = ~(-1L << workerIdBits);
        var tasks = new ConcurrentBag<Task>();
        ThreadPool.GetMinThreads(out int workerThreads, out var minIoc);
        ThreadPool.SetMinThreads((int)maxWorkerId, minIoc);

        int laterTime = 0;
        try
        {
            Parallel.For(0, maxWorkerId * 3, _ =>
            {
                tasks.Add(GetWorkerIdAsync(null, workerIdBits));
            });
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("please try again later") ||
                (ex.InnerException != null && ex.InnerException.Message.Contains("please try again later")))
            {
                laterTime++;
            }
        }
        Assert.IsTrue(laterTime > 0);
        ThreadPool.SetMinThreads(workerThreads, minIoc);
    }

    [TestMethod]
    public void TestBaseRedis()
    {
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            },
            DefaultDatabase = 2
        };
        var baseRedis = new BaseRedis(redisConfigurationOptions);

        var dataBase = ((IDatabase)typeof(BaseRedis).GetProperty("Database", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(baseRedis)!);
        Assert.AreEqual(2, dataBase.Database);
    }

    [TestMethod]
    public async Task TestGetWorkerIdByLogOutAsync()
    {
        var services = new ServiceCollection();
        var workerProvider = (CustomDistributedWorkerProvider)GetWorkerProvider(services);

        _database.KeyDelete(_logOutWorkerKey);
        var workerId = await workerProvider.TestGetWorkerIdByLogOutAsync();
        Assert.IsNull(workerId);

        await _database.SortedSetAddAsync(_logOutWorkerKey, 10, GetCurrentTimestamp());
        workerId = await workerProvider.TestGetWorkerIdByLogOutAsync();
        Assert.AreEqual(10, workerId);
    }

    [TestMethod]
    public void TestTilNextMillis()
    {
        var workerProvider = Substitute.For<IWorkerProvider>();

        int workerId = 1;
        workerProvider.GetWorkerIdAsync().Returns(workerId);
        var redisConfigurationOptions = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            }
        };
        var distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(new SnowflakeGeneratorOptions(new ServiceCollection())
        {
            TimestampType = TimestampType.Milliseconds
        })
        {
            RefreshTimestampInterval = 500
        };
        var machineClockIdGenerator = new CustomMachineClockIdGenerator(
            workerProvider,
            redisConfigurationOptions,
            distributedIdGeneratorOptions
        );
        long lastTimestamp = 500;
        var result = machineClockIdGenerator.TestTilNextMillis(lastTimestamp);
        Assert.AreEqual(501, result);

        var dataBase = ConnectionMultiplexer.Connect(redisConfigurationOptions).GetDatabase(redisConfigurationOptions.DefaultDatabase);
        string lastTimestampKey = "snowflake.last_timestamp";
        Assert.AreEqual(result, dataBase.HashGet(lastTimestampKey, workerId));

        dataBase.HashSet(lastTimestampKey, workerId, 10);

        distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(new SnowflakeGeneratorOptions(new ServiceCollection())
        {
            TimestampType = TimestampType.Seconds
        })
        {
            RefreshTimestampInterval = 5000
        };
        machineClockIdGenerator = new CustomMachineClockIdGenerator(
            workerProvider,
            redisConfigurationOptions,
            distributedIdGeneratorOptions
        );
        result = machineClockIdGenerator.TestTilNextMillis(1);
        Assert.AreEqual(2, result);
        Assert.AreEqual(10, dataBase.HashGet(lastTimestampKey, workerId));
        dataBase.HashDelete(lastTimestampKey, workerId);
    }

    #region private methods

    private Task<long> GetWorkerIdAsync(IServiceCollection? services, int workerIdBits)
        => GetWorkerProvider(services, workerIdBits).GetWorkerIdAsync();

    private IWorkerProvider GetWorkerProvider(IServiceCollection? services, int workerIdBits = 10)
    {
        var snowflakeGeneratorOptions = new SnowflakeGeneratorOptions(services ?? new ServiceCollection())
        {
            WorkerIdBits = workerIdBits
        };
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(snowflakeGeneratorOptions)
        {
            GetWorkerIdMinInterval = 0
        };

        return new CustomDistributedWorkerProvider(distributedIdGeneratorOptions, _redisOptions.Value, null);
    }

    private static long GetCurrentTimestamp(DateTime? dateTime = null)
        => new DateTimeOffset(dateTime ?? DateTime.UtcNow).ToUnixTimeMilliseconds();

    #endregion

}
#pragma warning restore CS0618
