// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.Tests;

[TestClass]
public class IdGeneratorTest
{
    private RedisCacheClient _redisCacheClient;
    private IOptions<RedisConfigurationOptions> _redisOptions;

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestInitialize]
    public async Task InitRedisDataAsync()
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
        db.KeyDelete("snowflake.current.workerid");
        db.KeyDelete("snowflake.inuse.workerid");
        db.KeyDelete("snowflake.logout.workerid");
        db.KeyDelete("snowflake.get.workerid");
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
            services.AddSnowflake(options =>
            {
                options.UseRedis();
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

        services.AddSnowflake(snowflakeGeneratorOptions =>
        {
            snowflakeGeneratorOptions.UseRedis();
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

        Assert.IsTrue(ids.Distinct().Count() == ids.Count);
    }

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

        services.AddSnowflake(snowflakeGeneratorOptions =>
        {
            snowflakeGeneratorOptions.UseRedis(distributedIdGeneratorOptions =>
            {
                distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0;
            });
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

        services.AddSnowflake(snowflakeGeneratorOptions =>
        {
            snowflakeGeneratorOptions.UseRedis(distributedIdGeneratorOptions => distributedIdGeneratorOptions.GetWorkerIdMinInterval = 0);
            snowflakeGeneratorOptions.EnableMachineClock = true;
        });

        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<long>>();
        var id = idGenerator.NewId();
        var maxSequenceBit = ~(-1L << 12);
        for (int i = 1; i < maxSequenceBit; i++)
        {
            var idTemp = idGenerator.NewId();
            Assert.IsTrue(i + id == idTemp);
        }
    }

    /// <summary>
    /// Only supports local testing
    /// </summary>
    [TestMethod]
    public async Task TestGetWorkerIdAsync()
    {
        var workerIdProvider = GetWorkerProvider(null);
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

    /// <summary>
    /// Only supports local testing
    /// </summary>
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
    }

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
        return new CustomDistributedWorkerProvider(_redisCacheClient, distributedIdGeneratorOptions, _redisOptions, null);
    }

    private static ConfigurationOptions GetConfigurationOptions(RedisConfigurationOptions redisOptions)
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
