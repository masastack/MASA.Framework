using Masa.Utils.Caching.Redis;

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests;

[TestClass]
public class IdGeneratorTest
{
    private readonly string _currentWorkerKey;
    private readonly string _inUseWorkerKey;
    private readonly string _logOutWorkerKey;
    private readonly string _getWorkerIdKey;

    public IdGeneratorTest()
    {
        _currentWorkerKey = "snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";
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
        return;

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
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake,long>>();
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
        return;

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

        var redisOptions = services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();
        var options = GetConfigurationOptions(redisOptions.CurrentValue);
        var connection = await ConnectionMultiplexer.ConnectAsync(options);
        var db = connection.GetDatabase(options.DefaultDatabase ?? 0);
        db.KeyDelete(_currentWorkerKey);
        db.KeyDelete(_inUseWorkerKey);
        db.KeyDelete(_logOutWorkerKey);
        db.KeyDelete(_getWorkerIdKey);

        services.AddDistributedSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0;
        });

        var workerIdProvider = services.BuildServiceProvider().GetRequiredService<IWorkerProvider>();
        List<long> workerIds = new();
        var maxWorkerId = ~(-1L << 10);
        for (int index = 0; index <= maxWorkerId; index++)
        {
            var workerId = await workerIdProvider.GetWorkerIdAsync();
            await workerIdProvider.LogOutAsync();
            workerIds.Add(workerId);
        }

        Assert.IsTrue(workerIds.Distinct().Count() + 1 == workerIds.Count());
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public async Task TestDistributedWorkerAndEnableMachineClockAsync()
    {
        return;

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

        var redisOptions = services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();
        var options = GetConfigurationOptions(redisOptions.CurrentValue);
        var connection = await ConnectionMultiplexer.ConnectAsync(options);
        var db = connection.GetDatabase(options.DefaultDatabase ?? 0);
        db.KeyDelete(_currentWorkerKey);
        db.KeyDelete(_inUseWorkerKey);
        db.KeyDelete(_logOutWorkerKey);
        db.KeyDelete(_getWorkerIdKey);

        services.AddDistributedSnowflake(distributedIdGeneratorOptions =>
        {
            distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0;
            distributedIdGeneratorOptions.EnableMachineClock = true;
        });

        var idGenerator = services.BuildServiceProvider().GetRequiredService<IIdGenerator<System.Snowflake,long>>();
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
    public async Task TestGetWorkerIdAsync()
    {
        // return;

        IServiceCollection services = new ServiceCollection();
        services.Configure<RedisConfigurationOptions>(option =>
        {
            option.Password = "";
            option.DefaultDatabase = 2;
            option.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });
        var serviceProvider = services.BuildServiceProvider();
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions()
        {
            GetWorkerIdMinInterval = 0
        };
        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
        var options = GetConfigurationOptions(redisOptions.Value);
        var connection = await ConnectionMultiplexer.ConnectAsync(options);
        var db = connection.GetDatabase(options.DefaultDatabase ?? 0);
        db.KeyDelete(_currentWorkerKey);
        db.KeyDelete(_inUseWorkerKey);
        db.KeyDelete(_logOutWorkerKey);
        db.KeyDelete(_getWorkerIdKey);
        var workerIdProvider = new CustomizeDistributedWorkerProvider(new RedisCacheClient(options),distributedIdGeneratorOptions, redisOptions, null);

        List<long> workerIds = new();
        var errCount = 0;
        var maxWorkerId = ~(-1L << 10);
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
