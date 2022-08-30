﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class DistributedCacheClient : BaseDistributedCacheClient, IDistributedCacheClient
{
    public DistributedCacheClient(IOptionsMonitor<RedisConfigurationOptions> redisConfigurationOptions,
        string name,
        JsonSerializerOptions? jsonSerializerOptions = null)
        : this(redisConfigurationOptions.Get(name), jsonSerializerOptions)
    {
        redisConfigurationOptions.OnChange((option, optionName) =>
        {
            //todo:
            if (optionName == name)
            {
                RefreshRedisConfigurationOptions(option);
            }
        });
    }

    public DistributedCacheClient(RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
        : base(redisConfigurationOptions, jsonSerializerOptions)
    {
    }

    #region Get

    public T? Get<T>(string key)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        return GetAndRefresh<T>(key);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        return await GetAndRefreshAsync<T>(key);
    }

    public IEnumerable<T?> GetList<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var list = GetList(keys, true);

        RefreshCore(list);

        return list.Select(option => ConvertToValue<T>(option.Value)).ToList();
    }

    public async Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var list = await GetListAsync(keys, true);

        await RefreshCoreAsync(list);

        return list.Select(option => ConvertToValue<T>(option.Value)).ToList();
    }

    public T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        return GetAndRefresh(key, () =>
        {
            var cacheEntry = setter();
            Set(key, cacheEntry.Value, cacheEntry);
            return cacheEntry.Value;
        });
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        return await GetAndRefreshAsync(key, async () =>
        {
            var cacheEntry = setter();
            await SetAsync(key, cacheEntry.Value, cacheEntry);
            return cacheEntry.Value;
        });
    }

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">keyPattern, such as: app_*</param>
    /// <returns></returns>
    public List<string> GetKeys(string keyPattern)
    {
        var prepared = LuaScript.Prepare(Const.GET_KEYS_SCRIPT);
        var cacheResult = Db.ScriptEvaluate(prepared, new { pattern = keyPattern });
        return ((string[])cacheResult).ToList();
    }

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">keyPattern, such as: app_*</param>
    /// <returns></returns>
    public async Task<List<string>> GetKeysAsync(string keyPattern)
    {
        var prepared = LuaScript.Prepare(Const.GET_KEYS_SCRIPT);
        var cacheResult = await Db.ScriptEvaluateAsync(prepared, new { pattern = keyPattern });
        return ((string[])cacheResult).ToList();
    }

    public List<KeyValuePair<string, T?>> GetListByKeyPattern<T>(string keyPattern)
    {
        var list = GetListByKeyPattern(keyPattern);

        RefreshCore(list);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value))).ToList();
    }

    public async Task<List<KeyValuePair<string, T?>>> GetListByKeyPatternAsync<T>(string keyPattern)
    {
        var list = await GetListByKeyPatternAsync(keyPattern);

        await RefreshCoreAsync(list);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value))).ToList();
    }

    #endregion

    #region Set

    public void Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
        => Set(key, value, new CacheEntryOptions<T>(absoluteExpiration));

    public Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration)
        => SetAsync(key, value, new CacheEntryOptions<T>(absoluteExpiration));

    public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        => Set(key, value, new CacheEntryOptions<T>(absoluteExpirationRelativeToNow));

    public Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        => SetAsync(key, value, new CacheEntryOptions<T>(absoluteExpirationRelativeToNow));

    public void Set<T>(string key, T value, CacheEntryOptions<T>? options = null)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var bytesValue = value.ConvertFromValue(JsonSerializerOptions);

        Db.ScriptEvaluate(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue }));
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions<T>? options = null)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var bytesValue = value.ConvertFromValue(JsonSerializerOptions);

        await Db.ScriptEvaluateAsync(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue })
        ).ConfigureAwait(false);
    }

    public void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
        => SetList(keyValues, new CacheEntryOptions<T>(absoluteExpiration));

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
        => SetListAsync(keyValues, new CacheEntryOptions<T>(absoluteExpiration));

    public void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
        => SetList(keyValues, new CacheEntryOptions<T>(absoluteExpirationRelativeToNow));

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
        => SetListAsync(keyValues, new CacheEntryOptions<T>(absoluteExpirationRelativeToNow));

    public void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(keyValues, nameof(keyValues));

        var redisKeys = keyValues.Select(item => (RedisKey)item.Key).ToArray();
        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(JsonSerializerOptions)).ToArray();

        Db.ScriptEvaluate(
            Const.SET_MULTIPLE_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        );
    }

    public async Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(keyValues, nameof(keyValues));

        var redisKeys = keyValues.Select(item => (RedisKey)item.Key).ToArray();
        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(JsonSerializerOptions)).ToArray();

        await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        );
    }

    #endregion

    #region Refresh

    public void Refresh(params string[] keys)
    {
        var list = GetList(keys, false);

        RefreshCore(list);
    }

    public async Task RefreshAsync(params string[] keys)
    {
        var list = await GetListAsync(keys, false);

        await RefreshCoreAsync(list);
    }

    #endregion

    #region Remove

    public void Remove(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        Db.KeyDelete(keys.Select(key => (RedisKey)key).ToArray());

        Parallel.ForEach(keys, key => Publish(key, options =>
        {
            options.Operation = SubscribeOperation.Remove;
            options.Key = key;
        }));
    }

    public Task RemoveAsync(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        return Db.KeyDeleteAsync(keys.Select(key => (RedisKey)key).ToArray());
    }

    #endregion

    #region Exist

    public bool Exists(string key)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        return Db.KeyExists(key);
    }

    public Task<bool> ExistsAsync(string key)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        return Db.KeyExistsAsync(key);
    }

    #endregion

    #region PubSub

    public void Publish(string channel, Action<PublishOptions> setup)
    {
        PublishCore(channel, setup, (c, message) =>
        {
            Subscriber.Publish(c, message);
            return Task.CompletedTask;
        });
    }

    public async Task PublishAsync(string channel, Action<PublishOptions> setup)
    {
        PublishCore(channel, setup, async (c, message) => await Subscriber.PublishAsync(c, message));
        await Task.CompletedTask;
    }

    public void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        Subscriber.Subscribe(channel, (_, message) =>
        {
            var options = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            if (options != null)
                options.IsPublishClient = options.UniquelyIdentifies == UniquelyIdentifies;
            handler(options!);
        });
    }

    public Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        return Subscriber.SubscribeAsync(channel, (_, message) =>
        {
            var options = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            if (options != null)
                options.IsPublishClient = options.UniquelyIdentifies == UniquelyIdentifies;
            handler(options!);
        });
    }

    #endregion

    #region Hash

    public Task<long> HashIncrementAsync(string key, long value = 1)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        return Db.HashIncrementAsync(key, Const.DATA_KEY, value);
    }

    public async Task<long> HashDecrementAsync(string key, long value = 1, long defaultMinVal = 0L)
    {
        if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        if (defaultMinVal < 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than or equal to 0");

        var script = $@"
local result = redis.call('HGET', KEYS[1], KEYS[2])
if tonumber(result) > {defaultMinVal} then
    result = redis.call('HINCRBY', KEYS[1], KEYS[2], {0 - value})
    return result
else
    return -1
end";
        var result = (long)await Db.ScriptEvaluateAsync(script, new RedisKey[] { key, Const.DATA_KEY });

        return result;
    }

    #endregion

    #region Expire

    public bool KeyExpire(string key, DateTimeOffset absoluteExpiration)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpiration));

    public Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpiration));

    public bool KeyExpire(string key, TimeSpan absoluteExpirationRelativeToNow)
        => KeyExpire(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public Task<bool> KeyExpireAsync(string key, TimeSpan absoluteExpirationRelativeToNow)
        => KeyExpireAsync(key, new CacheEntryOptions(absoluteExpirationRelativeToNow));

    public bool KeyExpire(string key, CacheEntryOptions? options = null)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        var result = Db.ScriptEvaluate(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );

        return (long?)result == 1;
    }

    public long KeyExpire(string[] keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var redisKeys = keys.Select(key => (RedisKey)key).ToArray();

        var result = Db.ScriptEvaluate(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options))
        );

        return (long)result;
    }

    public async Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
    {
        key.CheckIsNullOrWhiteSpace(nameof(key));

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );

        return (long)result == 1;
    }

    public async Task<long> KeyExpireAsync(string[] keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var redisKeys = keys.Select(key => (RedisKey)key).ToArray();

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options))
        );

        return (long)result;
    }

    #endregion

    #region private methods

    private void RefreshRedisConfigurationOptions(RedisConfigurationOptions redisConfigurationOptions)
    {
        IConnectionMultiplexer? connection = ConnectionMultiplexer.Connect(redisConfigurationOptions);
        Db = connection.GetDatabase();
        Subscriber = connection.GetSubscriber();

        CacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpiration = redisConfigurationOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = redisConfigurationOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = redisConfigurationOptions.SlidingExpiration
        };
    }

    private RedisValue[] GetRedisValues(CacheEntryOptions? options, Func<RedisValue[]>? func = null)
    {
        var creationTime = DateTimeOffset.UtcNow;
        var cacheEntryOptions = GetCacheEntryOptions(options);
        var absoluteExpiration = cacheEntryOptions.GetAbsoluteExpiration(creationTime);
        List<RedisValue> redisValues = new()
        {
            absoluteExpiration?.Ticks ?? Const.DEADLINE_LASTING,
            cacheEntryOptions.SlidingExpiration?.Ticks ?? Const.DEADLINE_LASTING,
            TimeHelper.GetExpirationInSeconds(creationTime, absoluteExpiration, cacheEntryOptions.SlidingExpiration) ??
            Const.DEADLINE_LASTING,
        };
        if (func != null)
            redisValues.AddRange(func.Invoke());

        return redisValues.ToArray();
    }

    private T? GetAndRefresh<T>(string key, Func<T>? func = null, CommandFlags flags = CommandFlags.None)
    {
        var results = Db.HashMemberGet(
            key,
            Const.ABSOLUTE_EXPIRATION_KEY,
            Const.SLIDING_EXPIRATION_KEY,
            Const.DATA_KEY);

        var result = GetByArrayRedisValue<T>(results, key);
        if (result.Value != null)
            Refresh(result.DataCacheOptions, flags);
        else if (func != null)
            result.Value = func.Invoke();

        return result.Value;
    }

    private async Task<T?> GetAndRefreshAsync<T>(string key, Func<Task<T>>? func = null, CommandFlags flags = CommandFlags.None)
    {
        var results = await Db.HashMemberGetAsync(
            key,
            Const.ABSOLUTE_EXPIRATION_KEY,
            Const.SLIDING_EXPIRATION_KEY,
            Const.DATA_KEY);

        var result = GetByArrayRedisValue<T>(results, key);

        if (result.Value != null)
            await RefreshAsync(result.DataCacheOptions, flags);
        else if (func != null)
            result.Value = await func.Invoke();

        return result.Value;
    }

    private (T? Value, DataCacheOptions DataCacheOptions) GetByArrayRedisValue<T>(
        RedisValue[] redisValue,
        string key)
    {
        var options = MapMetadata(key, redisValue);
        var value = ConvertToValue<T>(options.Value);
        return (value, options);
    }

    #region Refresh

    private void Refresh(DataCacheOptions dataCacheOptions, CommandFlags flags)
    {
        var result = dataCacheOptions.GetExpiration();
        if (result.State) Db.KeyExpire(dataCacheOptions.Key, result.Expire, flags);
    }

    private async Task RefreshAsync(
        DataCacheOptions dataCacheOptions,
        CommandFlags flags,
        CancellationToken token = default)
    {
        var result = dataCacheOptions.GetExpiration(null, token);
        if (result.State) await Db.KeyExpireAsync(dataCacheOptions.Key, result.Expire, flags).ConfigureAwait(false);
    }

    #endregion

    #endregion

}