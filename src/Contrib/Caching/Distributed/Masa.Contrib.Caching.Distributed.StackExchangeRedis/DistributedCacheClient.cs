// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class DistributedCacheClient : BaseDistributedCacheClient, IDistributedCacheClient
{
    private SubscribeConfigurationOptions _subscribeConfigurationOptions;
    private readonly object _locker = new();
    private readonly IList<string> _subscribeChannels = new List<string>();

    public DistributedCacheClient(
        IOptionsMonitor<DistributedRedisCacheOptions> defaultOptionsMonitor,
        DistributedRedisCacheOptions? cacheOptions)
        : base(defaultOptionsMonitor, cacheOptions)
    {
        defaultOptionsMonitor.OnChange(option =>
        {
            if (cacheOptions == null || cacheOptions.SubscribeConfigurationOptions == null)
                _subscribeConfigurationOptions = option.SubscribeConfigurationOptions!;
        });
        _subscribeConfigurationOptions = cacheOptions?.SubscribeConfigurationOptions ??
            defaultOptionsMonitor.CurrentValue.SubscribeConfigurationOptions!;
    }

    #region Get

    public T? Get<T>(string key)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        return GetAndRefresh<T>(key);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        return await GetAndRefreshAsync<T>(key);
    }

    public T? Get<T>(string key, Action<T?> valueChanged)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var value = Get<T>(key);

        if (value == null)
            return value;

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe(channel, new CombinedCacheEntryOptions<T>
        {
            ValueChanged = valueChanged
        });
        return value;
    }

    public async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));
        var value = await GetAsync<T>(key);

        if (value == null)
            return value;

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe(channel, new CombinedCacheEntryOptions<T>
        {
            ValueChanged = valueChanged
        });
        return value;
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
        CheckIsNullOrWhiteSpace(key, nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        return GetAndRefresh<T>(key, () =>
        {
            var cacheEntry = setter();
            Set(key, cacheEntry.Value, cacheEntry);
        });
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        return await GetAndRefreshAsync<T>(key, async () =>
        {
            var cacheEntry = setter();
            await SetAsync(key, cacheEntry.Value, cacheEntry);
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
        if (cacheResult.IsNull)
            return new List<string>();

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
        if (cacheResult.IsNull) return new List<string>();

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
        CheckIsNullOrWhiteSpace(key, nameof(key));

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var bytesValue = value.ConvertFromValue(JsonSerializerOptions);

        Db.ScriptEvaluate(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue }));
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions<T>? options = null)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

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
        CheckIsNullOrWhiteSpace(key, nameof(key));

        return Db.KeyExists(key);
    }

    public Task<bool> ExistsAsync(string key)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

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
            handler(options!);
        });
    }

    public Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        return Subscriber.SubscribeAsync(channel, (_, message) =>
        {
            var options = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
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
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

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
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var result = Db.ScriptEvaluate(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );
        if (result.IsNull)
            return false;

        return (long?)result == 1;
    }

    public bool KeyExpire(string[] keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var redisKeys = keys.Select(key => (RedisKey)key).ToArray();

        var result = Db.ScriptEvaluate(
            Const.SET_MULTIPLE_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options))
        );

        if (result.IsNull)
            return false;

        return (long?)result == 1;
    }

    public async Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );
        if (result.IsNull)
            return false;

        return (long?)result == 1;
    }

    public async Task<bool> KeyExpireAsync(string[] keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var redisKeys = keys.Select(key => (RedisKey)key).ToArray();

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_SCRIPT,
            redisKeys,
            GetRedisValues(GetCacheEntryOptions(options))
        );

        if (result.IsNull)
            return false;

        return (long?)result == 1;
    }

    #endregion

    #region private methods

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

    private T? GetAndRefresh<T>(string key, Action? action = null, CommandFlags flags = CommandFlags.None)
    {
        var results = Db.HashMemberGet(
            key,
            Const.ABSOLUTE_EXPIRATION_KEY,
            Const.SLIDING_EXPIRATION_KEY,
            Const.DATA_KEY);

        var result = GetByArrayRedisValue<T>(results, key);
        if (result.Value != null)
            Refresh(result.DataCacheOptions, flags);
        else if (action != null)
            action.Invoke();

        return result.Value;
    }

    private async Task<T?> GetAndRefreshAsync<T>(string key, Func<Task>? func = null, CommandFlags flags = CommandFlags.None)
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
            await func.Invoke();

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

    private string FormatSubscribeChannel<T>(string key) =>
        SubscribeHelper.FormatSubscribeChannel<T>(key,
            _subscribeConfigurationOptions.SubscribeKeyTypes,
            _subscribeConfigurationOptions.SubscribeKeyPrefix);

    private void Subscribe<T>(string channel, CombinedCacheEntryOptions<T>? options = null)
    {
        if (_subscribeChannels.Contains(channel))
            return;

        lock (_locker)
        {
            if (_subscribeChannels.Contains(channel))
                return;

            Subscribe<T>(channel, (subscribeOptions) =>
            {
                switch (subscribeOptions.Operation)
                {
                    case SubscribeOperation.Set:
                    case SubscribeOperation.Remove:
                        break;
                    default:
                        throw new NotImplementedException();
                }

                options?.ValueChanged?.Invoke(subscribeOptions.Value);
            });

            _subscribeChannels.Add(channel);
        }
    }

    #endregion

}
