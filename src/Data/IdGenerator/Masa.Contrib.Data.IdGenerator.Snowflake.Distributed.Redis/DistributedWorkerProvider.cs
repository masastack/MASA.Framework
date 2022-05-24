// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedWorkerProvider : IWorkerProvider
{
    private long _workerId;
    private readonly bool _enableRecycle;
    private readonly long _recycleTime;
    private readonly long _maxWorkerId;
    private readonly string _currentWorkerKey;
    private readonly string _inUseWorkerKey;
    private readonly string _logOutWorkerKey;
    private readonly string _getWorkerIdKey;
    private readonly string _token;
    private readonly TimeSpan _timeSpan;
    public bool SupportDistributed => true;
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;

    public DistributedWorkerProvider(DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        IOptionsMonitor<RedisConfigurationOptions> redisOptions)
    {
        ArgumentNullException.ThrowIfNull(distributedIdGeneratorOptions);

        _enableRecycle = distributedIdGeneratorOptions.EnableRecycle;
        _recycleTime = distributedIdGeneratorOptions.RecycleTime;
        _maxWorkerId = distributedIdGeneratorOptions.MaxWorkerId;
        _currentWorkerKey = "snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";
        _token = Environment.MachineName;
        _timeSpan = TimeSpan.FromSeconds(10);
        var options = GetConfigurationOptions(redisOptions.CurrentValue);
        _connection = ConnectionMultiplexer.Connect(options);
        _database = _connection.GetDatabase(options.DefaultDatabase ?? 0);
    }

    public async Task<long> GetWorkerIdAsync()
    {
        _workerId = await _database.StringIncrementAsync(_currentWorkerKey, 1) - 1;
        if (_workerId > _maxWorkerId)
        {
            var lockdb = _connection.GetDatabase(-1);
            if (lockdb.LockTake(_getWorkerIdKey, _token, _timeSpan))
            {
                try
                {
                    _workerId = await GetNextWorkerIdAsync();
                    await RefreshAsync();
                }
                finally
                {
                    lockdb.LockRelease(_getWorkerIdKey, _token);
                }
            }
        }
        else
        {
            await RefreshAsync();
        }
        return _workerId;
    }

    public Task RefreshAsync() =>
        _database.SortedSetAddAsync(_inUseWorkerKey, _workerId + "", GetCurrentTimestamp());

    public async Task LogOutAsync()
    {
        var val = _workerId + "";
        await _database.SortedSetAddAsync(_logOutWorkerKey, val, GetCurrentTimestamp());
        await _database.SortedSetRemoveAsync(_inUseWorkerKey, val);
    }

    private async Task<long> GetNextWorkerIdAsync()
    {
        var workerId = await GetWorkerIdByLogOutAsync();
        if (workerId != null)
            return workerId.Value;

        if (!_enableRecycle) throw new MasaException("No WorkerId available");

        workerId = await GetWorkerIdByInUseAsync();
        if (workerId != null)
            return workerId.Value;

        throw new MasaException("No WorkerId available");
    }

    private async Task<long?> GetWorkerIdByLogOutAsync()
    {
        var entries = await _database.SortedSetRangeByScoreWithScoresAsync(_logOutWorkerKey, take: 1);
        if (entries is { Length: > 0 })
            return long.Parse(entries[0].Element);

        return null;
    }

    private async Task<long?> GetWorkerIdByInUseAsync()
    {
        var entries = await _database.SortedSetRangeByScoreWithScoresAsync(
            _inUseWorkerKey,
            0,
            GetCurrentTimestamp(DateTime.UtcNow.AddMilliseconds(-_recycleTime)),
            take: 1);

        if (entries is { Length: > 0 })
            return long.Parse(entries[0].Element);

        return null;
    }

    private long GetCurrentTimestamp(DateTime? dateTime = null) => new DateTimeOffset(dateTime ?? DateTime.UtcNow).ToUnixTimeMilliseconds();

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
