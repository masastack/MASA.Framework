// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedWorkerProvider : BaseRedis, IWorkerProvider
{
    private readonly string _channel = "snowflake.workerid";
    private long? _workerId;
    private readonly TimestampType _timestampType;
    private readonly long _idleTimeOut;
    private readonly long _maxWorkerId;
    private readonly long _workerIdMinInterval;
    private readonly string _currentWorkerKey;
    private readonly string _inUseWorkerKey;
    private readonly string _logOutWorkerKey;
    private readonly string _getWorkerIdKey;
    private readonly string _token;
    private readonly TimeSpan _timeSpan;
    private DateTime? _lastTime;
    private readonly ILogger<DistributedWorkerProvider>? _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly string? _uniquelyIdentifies;

    public DistributedWorkerProvider(
        DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        RedisConfigurationOptions redisOptions,
        ILogger<DistributedWorkerProvider>? logger) : base(redisOptions)
    {
        _uniquelyIdentifies ??= Guid.NewGuid().ToString();
        ArgumentNullException.ThrowIfNull(distributedIdGeneratorOptions);

        _timestampType = distributedIdGeneratorOptions.IdGeneratorOptions.TimestampType;
        _idleTimeOut = distributedIdGeneratorOptions.IdleTimeOut;
        _maxWorkerId = distributedIdGeneratorOptions.IdGeneratorOptions.MaxWorkerId;
        _workerIdMinInterval = distributedIdGeneratorOptions.GetWorkerIdMinInterval;
        _currentWorkerKey = "snowflake.current.workerid";
        _inUseWorkerKey = "snowflake.inuse.workerid";
        _logOutWorkerKey = "snowflake.logout.workerid";
        _getWorkerIdKey = "snowflake.get.workerid";
        _token = Environment.MachineName;
        _timeSpan = TimeSpan.FromSeconds(10);
        _logger = logger;

        DistributedCacheClient.Subscribe<long>(_channel, options =>
        {
            if (options.Key == _uniquelyIdentifies)
                return;

            if (_workerId.HasValue && _workerId.Value == options.Value)
                _workerId = null;
        });
    }

    public async Task<long> GetWorkerIdAsync()
    {
        if (_workerId.HasValue)
            return _workerId.Value;

        if (_lastTime != null && (DateTime.UtcNow - _lastTime.Value).TotalMilliseconds - _workerIdMinInterval < 0)
        {
            _logger?.LogDebug("Failed to get WorkerId, please rest for a while and try again");
            throw new MasaException("Failed to get WorkerId, please rest for a while and try again");
        }

        _lastTime = DateTime.UtcNow;
        await _semaphore.WaitAsync();
        try
        {
            if (_workerId.HasValue)
                return _workerId.Value;

            _workerId = await GetNextWorkerIdAsync();

            await RefreshAsync();

            await DistributedCacheClient.PublishAsync(_channel, options =>
            {
                options.Key = _uniquelyIdentifies!;
                options.Value = _workerId.Value;
            });
            return _workerId.Value;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RefreshAsync()
    {
        if (_workerId == null)
            throw new MasaException("No WorkerId available");

        await Database.SortedSetAddAsync(_inUseWorkerKey, _workerId, GetCurrentTimestamp());
    }

    public async Task LogOutAsync()
    {
        if (_workerId == null)
            return;

        var workerId = _workerId;
        _workerId = null;

        if (_logger?.IsEnabled(LogLevel.Debug) == true)
        {
            _logger.LogDebug("----- Logout WorkerId, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
                workerId,
                DateTime.UtcNow);
        }

        await Database.SortedSetAddAsync(_logOutWorkerKey, workerId, GetCurrentTimestamp());
        await Database.SortedSetRemoveAsync(_inUseWorkerKey, workerId);
        if (_logger?.IsEnabled(LogLevel.Debug) == true)
        {
            _logger.LogDebug("----- Logout WorkerId succeeded, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
            workerId,
            DateTime.UtcNow);
        }
    }

    private async Task<long> GetNextWorkerIdAsync()
    {
        var workerId = await DistributedCacheClient.HashIncrementAsync(_currentWorkerKey, 1) - 1;
        if (workerId > _maxWorkerId)
        {
            var lockdb = Database;
            if (await lockdb.LockTakeAsync(_getWorkerIdKey, _token, _timeSpan))
            {
                try
                {
                    workerId = await GetNextWorkerIdByDistributedLockAsync();
                }
                finally
                {
                    await lockdb.LockReleaseAsync(_getWorkerIdKey, _token);
                }
            }
            else
            {
                if (_logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    _logger.LogDebug(
                    "----- Failed to obtain WorkerId, failed to obtain distributed lock, the currentTime: {CurrentTime}",
                    DateTime.UtcNow);
                }
                throw new MasaException("----- Failed to get WorkerId, please try again later");
            }
        }
        else
        {
            _workerId = workerId;
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

    protected virtual async Task<long?> GetWorkerIdByLogOutAsync()
    {
        var entries = await Database.SortedSetRangeByScoreWithScoresAsync(_logOutWorkerKey, take: 1);
        if (entries is { Length: > 0 } && entries[0].Element.HasValue)
            return long.Parse(entries[0].Element.ToString());

        return null;
    }

    public async Task<long?> GetWorkerIdByInUseAsync()
    {
        var entries = await Database.SortedSetRangeByScoreWithScoresAsync(
            _inUseWorkerKey,
            0,
            GetCurrentTimestamp(DateTime.UtcNow.AddMilliseconds(-_idleTimeOut)),
            take: 1);

        if (entries is { Length: > 0 } && entries[0].Element.HasValue)
            return long.Parse(entries[0].Element.ToString());

        return null;
    }

    private long GetCurrentTimestamp(DateTime? dateTime = null)
        => new DateTimeOffset(dateTime ?? DateTime.UtcNow).GetTimestamp(_timestampType);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Connection.IsConnected || Connection.IsConnecting)
            base.Connection.Dispose();
    }
}
