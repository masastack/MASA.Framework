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
    private readonly object _lock = new();
    private readonly string? _uniquelyIdentifies;

    public DistributedWorkerProvider(
        IDistributedCacheClient distributedCacheClient,
        DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        RedisConfigurationOptions redisOptions,
        ILogger<DistributedWorkerProvider>? logger) : base(distributedCacheClient, redisOptions)
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

            RefreshAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            DistributedCacheClient.Publish(_channel, options =>
            {
                options.Key = _uniquelyIdentifies!;
                options.Value = _workerId.Value;
            });
            return Task.FromResult(_workerId.Value);
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

        _logger?.LogDebug("----- Logout WorkerId, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
            workerId,
            DateTime.UtcNow);

        await Database.SortedSetAddAsync(_logOutWorkerKey, workerId, GetCurrentTimestamp());
        await Database.SortedSetRemoveAsync(_inUseWorkerKey, workerId);

        _logger?.LogDebug("----- Logout WorkerId succeeded, the current WorkerId: {WorkerId}, currentTime: {CurrentTime}",
            workerId,
            DateTime.UtcNow);
    }

    private async Task<long> GetNextWorkerIdAsync()
    {
        var workerId = await DistributedCacheClient.HashIncrementAsync(_currentWorkerKey, 1) - 1;
        if (workerId > _maxWorkerId)
        {
            var lockdb = Connection.GetDatabase();
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
                _logger?.LogDebug(
                    "----- Failed to obtain WorkerId, failed to obtain distributed lock, the currentTime: {CurrentTime}",
                    DateTime.UtcNow);
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
        if (entries is { Length: > 0 })
            return long.Parse(entries[0].Element);

        return null;
    }

    public async Task<long?> GetWorkerIdByInUseAsync()
    {
        var entries = await Database.SortedSetRangeByScoreWithScoresAsync(
            _inUseWorkerKey,
            0,
            GetCurrentTimestamp(DateTime.UtcNow.AddMilliseconds(-_idleTimeOut)),
            take: 1);

        if (entries is { Length: > 0 })
            return long.Parse(entries[0].Element);

        return null;
    }

    private long GetCurrentTimestamp(DateTime? dateTime = null)
        => new DateTimeOffset(dateTime ?? DateTime.UtcNow).GetTimestamp(_timestampType);
}
