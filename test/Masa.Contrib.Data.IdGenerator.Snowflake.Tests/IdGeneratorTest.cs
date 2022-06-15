// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests;

[TestClass]
public class IdGeneratorTest
{
    private readonly string _currentWorkerKey;
    private readonly string _inUseWorkerKey;
    private readonly string _logOutWorkerKey;
    private readonly string _getWorkerIdKey;
    private RedisCacheClient _redisCacheClient;
    private IOptions<RedisConfigurationOptions> _redisOptions;

    public IdGeneratorTest()
    {
        _currentWorkerKey = "snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";
    }

    [TestInitialize]
    public async Task InitRedisData()
    {
        RedisConfigurationOptions redisConfigurationOptions = new()
        {
            Password = "",
            DefaultDatabase = 2,
            Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            }
        };
        _redisOptions = Options.Create(redisConfigurationOptions);
        var options = GetConfigurationOptions(_redisOptions.Value);
        _redisCacheClient = new RedisCacheClient(options);
        var connection = await ConnectionMultiplexer.ConnectAsync(options);
        var db = connection.GetDatabase(options.DefaultDatabase ?? 0);
        db.KeyDelete(_currentWorkerKey);
        db.KeyDelete(_inUseWorkerKey);
        db.KeyDelete(_logOutWorkerKey);
        db.KeyDelete(_getWorkerIdKey);
    }

    [TestMethod]
    public void TestEnableMachineClock()
    {
        var services = new ServiceCollection();
        services.AddSnowflake(opt =>
        {
            opt.EnableMachineClock = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Create();
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
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Create();
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
    public void TestErrorHeartbeatIntervalReturnThrowArgumentOutOfRangeException()
    {
        var services = new ServiceCollection();
        services.AddMasaRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            services.AddDistributedSnowflake(options =>
            {
                options.HeartbeatInterval = 99;
            });
        });
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public void TestDistributedSnowflake()
    {
        var services = new ServiceCollection();
        services.AddMasaRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });

        services.AddDistributedSnowflake();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Create();
            ids.Add(id);
            count++;
        }

        if (ids.Distinct().Count() != ids.Count)
            throw new Exception("duplicate id");
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public async Task TestDistributedWorkerAsync()
    {
        var services = new ServiceCollection();
        services.AddMasaRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });

        services.AddDistributedSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0;
        });

        var serviceProvider = services.BuildServiceProvider();
        var workerIdProvider = serviceProvider.GetRequiredService<IWorkerProvider>();
        List<long> workerIds = new();
        var maxWorkerId = ~(-1L << 10);
        for (int index = 0; index <= maxWorkerId; index++)
        {
            var workerId = await workerIdProvider.GetWorkerIdAsync();
            await workerIdProvider.LogOutAsync();
            workerIds.Add(workerId);
        }

        Assert.IsTrue(workerIds.Distinct().Count() == workerIds.Count && workerIds.Count == maxWorkerId + 1);
        // return Task.CompletedTask;
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public void TestDistributedWorkerAndEnableMachineClock()
    {
        var services = new ServiceCollection();
        services.AddMasaRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });

        services.AddDistributedSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0;
            distributedIdGeneratorOptions.EnableMachineClock = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        var id = idGenerator.Create();
        var maxSequenceBit = ~(-1L << 12);
        for (int i = 1; i < maxSequenceBit; i++)
        {
            var idTemp = idGenerator.Create();
            Assert.IsTrue(i + id == idTemp);
        }
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public Task TestGetWorkerIdAsync()
    {
        // IServiceCollection services = new ServiceCollection();
        // services.Configure<RedisConfigurationOptions>(option =>
        // {
        //     option.Password = "";
        //     option.DefaultDatabase = 2;
        //     option.Servers = new List<RedisServerOptions>()
        //     {
        //         new("127.0.0.1", 6379)
        //     };
        // });
        // var serviceProvider = services.BuildServiceProvider();
        // DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions()
        // {
        //     GetWorkerIdMinInterval = 0
        // };
        // var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
        // var options = GetConfigurationOptions(redisOptions.Value);
        // var connection = await ConnectionMultiplexer.ConnectAsync(options);
        // var db = connection.GetDatabase(options.DefaultDatabase ?? 0);
        // db.KeyDelete(_currentWorkerKey);
        // db.KeyDelete(_inUseWorkerKey);
        // db.KeyDelete(_logOutWorkerKey);
        // db.KeyDelete(_getWorkerIdKey);
        // var workerIdProvider = new CustomizeDistributedWorkerProvider(new RedisCacheClient(options), distributedIdGeneratorOptions, redisOptions, null);
        //
        // List<long> workerIds = new();
        // var errCount = 0;
        // var maxWorkerId = ~(-1L << 10);
        // for (int index = 0; index <= maxWorkerId + 1; index++)
        // {
        //     try
        //     {
        //         var workerId = await workerIdProvider.GetWorkerIdAsync();
        //         await workerIdProvider.LogOutAsync();
        //         workerIds.Add(workerId);
        //     }
        //     catch (MasaException ex)
        //     {
        //         errCount = 1;
        //         Assert.IsTrue(ex.Message == "No WorkerId available" && index == maxWorkerId + 1);
        //     }
        // }
        // Assert.IsTrue(workerIds.Count == maxWorkerId + 1);
        // Assert.IsTrue(errCount == 1);
        return Task.CompletedTask;
    }

    [TestMethod]
    public async Task TestGetDistibutedLockFaieldAsync()
    {
        var workerIdBits = 10;
        var maxWorkerId = ~(-1L << workerIdBits);
        var tasks = new ConcurrentBag<Task>();
        ThreadPool.GetMinThreads(out _, out var minIoc);
        ThreadPool.SetMinThreads((int)maxWorkerId, minIoc);

        int laterTime = 0;
        try
        {
            Parallel.For(0, maxWorkerId * 2, i =>
            {
                tasks.Add(GetWorkerIdAsync(workerIdBits));
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
    }

    private Task<long> GetWorkerIdAsync(int workerIdBits) => GetWorkerProvider(workerIdBits).GetWorkerIdAsync();

    private IWorkerProvider GetWorkerProvider(int workerIdBits = 10)
    {
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions()
        {
            WorkerIdBits = workerIdBits,
            GetWorkerIdMinInterval = 0
        };
        return new CustomizeDistributedWorkerProvider(_redisCacheClient, distributedIdGeneratorOptions, _redisOptions, null);
    }

    private ConfigurationOptions GetConfigurationOptions(RedisConfigurationOptions redisOptions)
    {
        var configurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = redisOptions.AbortOnConnectFail,
            AllowAdmin = redisOptions.AllowAdmin,
            ChannelPrefix = redisOptions.ChannelPrefix,
            ClientName = redisOptions.ClientName,
            ConnectRetry = redisOptions.ConnectRetry,
            ConnectTimeout = redisOptions.ConnectTimeout,
            DefaultDatabase = redisOptions.DefaultDatabase,
            Password = redisOptions.Password,
            Proxy = redisOptions.Proxy,
            Ssl = redisOptions.Ssl,
            SyncTimeout = redisOptions.SyncTimeout
        };

        foreach (var server in redisOptions.Servers)
        {
            configurationOptions.EndPoints.Add(server.Host, server.Port);
        }
        return configurationOptions;
    }
}
