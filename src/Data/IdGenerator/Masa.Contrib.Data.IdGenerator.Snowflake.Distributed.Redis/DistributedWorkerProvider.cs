// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedWorkerProvider : IWorkerProvider
{
    private long? _workerId;
    private readonly long _recycleTime;
    private readonly long _maxWorkerId;
    private readonly long _workerIdMinInterval;
    private readonly string _currentWorkerKey;
    private readonly string _inUseWorkerKey;
    private readonly string _logOutWorkerKey;
    private readonly string _getWorkerIdKey;
    private readonly string _token;
    private readonly TimeSpan _timeSpan;
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private DateTime? _lastTime;
    private readonly ILogger<DistributedWorkerProvider>? _logger;
    private readonly object _lock = new();

    public DistributedWorkerProvider(DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        IOptionsMonitor<RedisConfigurationOptions> redisOptions,
        ILogger<DistributedWorkerProvider>? logger)
    {
        ArgumentNullException.ThrowIfNull(distributedIdGeneratorOptions);

        _recycleTime = distributedIdGeneratorOptions.RecycleTime;
        _maxWorkerId = distributedIdGeneratorOptions.MaxWorkerId;
        _workerIdMinInterval = distributedIdGeneratorOptions.GetWorkerIdMinInterval;
        _currentWorkerKey = "snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";
        _token = Environment.MachineName;
        _timeSpan = TimeSpan.FromSeconds(10);
        var options = GetConfigurationOptions(redisOptions.CurrentValue);
        _connection = ConnectionMultiplexer.Connect(options);
        _database = _connection.GetDatabase(options.DefaultDatabase ?? 0);
        _logger = logger;
    }

    public Task<long> GetWorkerIdAsync()
    {
        if (_workerId.HasValue)
            return Task.FromResult(_workerId.Value);

        if (_lastTime != null && (DateTime.UtcNow - _lastTime.Value).TotalMilliseconds < _workerIdMinInterval)
        {
            _logger?.LogDebug("Failed to get WorkerId, please rest for a while and try again");
            throw new MasaException("Failed to get WorkerId, please rest for a while and try again");
        }

        _lastTime = DateTime.UtcNow;
        lock (_lock)
        {
            if (_workerId.HasValue)
                return Task.FromResult(_workerId.Value);

            _workerId = GetNextWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return Task.FromResult(_workerId.Value);
        }
    }

    public async Task RefreshAsync()
    {
        if (_workerId == null)
            return;

        await _database.SortedSetAddAsync(_inUseWorkerKey, _workerId + "", GetCurrentTimestamp());
    }

    public async Task LogOutAsync()
    {
        if (_workerId == null)
            return;

        var val = _workerId! + "";
        _logger?.LogDebug("----- Logout WorkerId, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
            _workerId,
            DateTime.UtcNow);

        _workerId = null;
        await _database.SortedSetAddAsync(_logOutWorkerKey, val, GetCurrentTimestamp());
        await _database.SortedSetRemoveAsync(_inUseWorkerKey, val);

        _logger?.LogDebug("----- Logout WorkerId succeeded, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
            _workerId,
            DateTime.UtcNow);
    }

    private async Task<long> GetNextWorkerIdAsync()
    {
        var workerId = await _database.StringIncrementAsync(_currentWorkerKey, 1) - 1;
        if (workerId > _maxWorkerId)
        {
            var lockdb = _connection.GetDatabase(-1);
            if (await lockdb.LockTakeAsync(_getWorkerIdKey, _token, _timeSpan))
            {
                try
                {
                    workerId = await GetNextWorkerIdByDistributedLockAsync();
                    await RefreshAsync();
                }
                finally
                {
                    await lockdb.LockReleaseAsync(_getWorkerIdKey, _token);
                }
            }
        }
        else
        {
            await RefreshAsync();
        }

        return workerId;
    }

    private async Task<long> GetNextWorkerIdByDistributedLockAsync()
    {
        var workerId = await GetWorkerIdByLogOutAsync();
        if (workerId != null)
            return workerId.Value;

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
