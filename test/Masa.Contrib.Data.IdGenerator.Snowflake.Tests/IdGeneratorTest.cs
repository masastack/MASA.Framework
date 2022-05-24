using System.Threading.Tasks;
using Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;
using Masa.Utils.Caching.Redis.DependencyInjection;
using Masa.Utils.Caching.Redis.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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
        Environment.SetEnvironmentVariable("WORKER_ID", "1");
        var services = new ServiceCollection();
        services.AddSnowflake(opt =>
        {
            opt.EnableMachineClock = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Generate();
            ids.Add(id);
            count++;
        }

        if (ids.Distinct().Count() != ids.Count)
            throw new Exception("duplicate id");
    }

    [TestMethod]
    public void TestDisableMachineClock()
    {
        Environment.SetEnvironmentVariable("WORKER_ID", "2");
        var services = new ServiceCollection();
        services.AddSnowflake();
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Generate();
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
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator>();
        int count = 1;
        List<long> ids = new();
        while (count < 500000)
        {
            var id = idGenerator.Generate();
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

        services.AddDistributedSnowflake();

        var workerProvider = services.BuildServiceProvider().GetRequiredService<IWorkerProvider>();
        List<long> workerIds = new();

        var maxWorkerId = ~(-1L << 10);
        var errCount = 0;
        for (int index = 0; index < maxWorkerId + 1; index++)
        {
            try
            {
                var workerId = await workerProvider.GetWorkerIdAsync();
                workerIds.Add(workerId);
            }
            catch (MasaException ex)
            {
                errCount = 1;
                Assert.IsTrue(ex.Message == "No WorkerId available" && index == maxWorkerId);
            }
        }
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
