// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class RedisCacheClient : RedisCacheClientBase
{
    public RedisCacheClient(IOptionsMonitor<RedisConfigurationOptions> redisConfigurationOptions,
        string name,
        JsonSerializerOptions? jsonSerializerOptions = null)
        : this(redisConfigurationOptions.Get(name), jsonSerializerOptions)
    {
        redisConfigurationOptions.OnChange((option, optionName) =>
        {
            if (optionName == name)
            {
                RefreshRedisConfigurationOptions(option);
            }
        });
    }

    public RedisCacheClient(RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null)
        : base(redisConfigurationOptions, jsonSerializerOptions)
    {
    }

    #region Get

    public override T? Get<T>(string key) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        return GetAndRefresh<T>(key);
    }

    public override async Task<T?> GetAsync<T>(string key) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        return await GetAndRefreshAsync<T>(key);
    }

    public override IEnumerable<T?> GetList<T>(IEnumerable<string> keys) where T : default
    {
        ArgumentNullException.ThrowIfNull(keys);

        var list = GetList(keys, true);

        RefreshCore(list);

        return list.Select(option => ConvertToValue<T>(option.Value, out _)).ToList();
    }

    public override async Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys) where T : default
    {
        ArgumentNullException.ThrowIfNull(keys);

        var list = await GetListAsync(keys, true);

        await RefreshCoreAsync(list);

        return list.Select(option => ConvertToValue<T>(option.Value, out _)).ToList();
    }

    public override T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(setter);

        return GetAndRefresh(key, () =>
        {
            var cacheEntry = setter();
            Set(key, cacheEntry.Value, cacheEntry);
            return cacheEntry.Value;
        });
    }

    public override async Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(setter);

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
    public override IEnumerable<string> GetKeys(string keyPattern)
    {
        var prepared = LuaScript.Prepare(Const.GET_KEYS_SCRIPT);
        var cacheResult = Db.ScriptEvaluate(prepared, new { pattern = keyPattern });
        return (string[])cacheResult;
    }

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">keyPattern, such as: app_*</param>
    /// <returns></returns>
    public override async Task<IEnumerable<string>> GetKeysAsync(string keyPattern)
    {
        var prepared = LuaScript.Prepare(Const.GET_KEYS_SCRIPT);
        var cacheResult = await Db.ScriptEvaluateAsync(prepared, new { pattern = keyPattern });
        return (string[])cacheResult;
    }

    public override IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(string keyPattern) where T : default
    {
        var list = GetListByKeyPattern(keyPattern);

        RefreshCore(list);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value, out _)));
    }

    public override async Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(string keyPattern) where T : default
    {
        var list = await GetListByKeyPatternAsync(keyPattern);

        await RefreshCoreAsync(list);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value, out _)));
    }

    #endregion

    #region Set

    public override void Set<T>(string key, T value, CacheEntryOptions? options = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(value);

        var bytesValue = value.ConvertFromValue(JsonSerializerOptions);

        Db.ScriptEvaluate(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue }));
    }

    public override async Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null)
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(value);

        var bytesValue = value.ConvertFromValue(JsonSerializerOptions);

        await Db.ScriptEvaluateAsync(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue })
        ).ConfigureAwait(false);
    }

    public override void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(JsonSerializerOptions)).ToArray();

        Db.ScriptEvaluate(
            Const.SET_MULTIPLE_SCRIPT,
            keyValues.Select(item => item.Key).GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        );
    }

    public override async Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(JsonSerializerOptions)).ToArray();

        await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_SCRIPT,
            keyValues.Select(item => item.Key).GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        );
    }

    #endregion

    #region Refresh

    public override void Refresh(IEnumerable<string> keys)
    {
        var list = GetList(keys, false);

        RefreshCore(list);
    }

    public override async Task RefreshAsync(IEnumerable<string> keys)
    {
        var list = await GetListAsync(keys, false);

        await RefreshCoreAsync(list);
    }

    #endregion

    #region Remove

    public override void Remove(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        Db.KeyDelete(keys.GetRedisKeys());

        Parallel.ForEach(keys, key => Publish(key, options =>
        {
            options.Operation = SubscribeOperation.Remove;
            options.Key = key;
        }));
    }

    public override Task RemoveAsync(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        return Db.KeyDeleteAsync(keys.GetRedisKeys());
    }

    #endregion

    #region Exist

    public override bool Exists(string key)
    {
        key.CheckIsNullOrWhiteSpace();

        return Db.KeyExists(key);
    }

    public override Task<bool> ExistsAsync(string key)
    {
        key.CheckIsNullOrWhiteSpace();

        return Db.KeyExistsAsync(key);
    }

    #endregion

    #region PubSub

    public override void Publish(string channel, Action<PublishOptions> options)
    {
        var publishOptions = GetAndCheckPublishOptions(channel, options);
        var message = JsonSerializer.Serialize(publishOptions, JsonSerializerOptions);
        Subscriber.Publish(channel, message);
    }

    public override async Task PublishAsync(string channel, Action<PublishOptions> options)
    {
        var publishOptions = GetAndCheckPublishOptions(channel, options);
        var message = JsonSerializer.Serialize(publishOptions, JsonSerializerOptions);
        await Subscriber.PublishAsync(channel, message);
    }

    public override void Subscribe<T>(string channel, Action<SubscribeOptions<T>> options)
    {
        Subscriber.Subscribe(channel, (_, message) =>
        {
            var subscribeOptions = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            if (subscribeOptions != null)
                subscribeOptions.IsPublisherClient = subscribeOptions.UniquelyIdentifies == UniquelyIdentifies;
            options(subscribeOptions!);
        });
    }

    public override Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> options)
    {
        return Subscriber.SubscribeAsync(channel, (_, message) =>
        {
            var subscribeOptions = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            if (subscribeOptions != null)
                subscribeOptions.IsPublisherClient = subscribeOptions.UniquelyIdentifies == UniquelyIdentifies;
            options(subscribeOptions!);
        });
    }

    #endregion

    #region Hash

    public override Task<long> HashIncrementAsync(string key, long value = 1)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        return Db.HashIncrementAsync(key, Const.DATA_KEY, value);
    }

    public override async Task<long> HashDecrementAsync(string key, long value = 1, long defaultMinVal = 0L)
    {
        CheckParametersByHashDecrement(value, defaultMinVal);

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

    private static void CheckParametersByHashDecrement(long value = 1, long defaultMinVal = 0L)
    {
        if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        if (defaultMinVal < 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than or equal to 0");
    }

    #endregion

    #region Expire

    public override bool KeyExpire(string key, CacheEntryOptions? options = null)
    {
        key.CheckIsNullOrWhiteSpace();

        var result = Db.ScriptEvaluate(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );

        return (long?)result == 1;
    }

    public override long KeyExpire(IEnumerable<string> keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = Db.ScriptEvaluate(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            keys.GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options))
        );

        return (long)result;
    }

    public override async Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
    {
        key.CheckIsNullOrWhiteSpace();

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );

        return (long)result == 1;
    }

    public override async Task<long> KeyExpireAsync(IEnumerable<string> keys, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            keys.GetRedisKeys(),
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
            DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, absoluteExpiration, cacheEntryOptions.SlidingExpiration) ??
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

        var result = GetByArrayRedisValue<T>(results, key, out bool isExist);

        if (isExist)
            Refresh(result.model, flags);
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

        var result = GetByArrayRedisValue<T>(results, key, out bool isExist);

        if (isExist)
            await RefreshAsync(result.model, flags);
        else if (func != null)
            result.Value = await func.Invoke();

        return result.Value;
    }

    private (T? Value, DataCacheModel model) GetByArrayRedisValue<T>(
        RedisValue[] redisValue,
        string key,
        out bool isExist)
    {
        var model = MapMetadata(key, redisValue);
        var value = ConvertToValue<T>(model.Value, out isExist);
        return (value, model);
    }

    #region Refresh

    private void Refresh(DataCacheModel model, CommandFlags flags)
    {
        var result = model.GetExpiration();
        if (result.State) Db.KeyExpire(model.Key, result.Expire, flags);
    }

    private async Task RefreshAsync(
        DataCacheModel model,
        CommandFlags flags,
        CancellationToken token = default)
    {
        var result = model.GetExpiration(null, token);
        if (result.State) await Db.KeyExpireAsync(model.Key, result.Expire, flags).ConfigureAwait(false);
    }

    #endregion

    #endregion

}
