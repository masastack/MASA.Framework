// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class RedisCacheClient : RedisCacheClientBase
{
    private readonly ITypeAliasProvider? _typeAliasProvider;

    public RedisCacheClient(IOptionsMonitor<RedisConfigurationOptions> redisConfigurationOptions,
        string name,
        JsonSerializerOptions? jsonSerializerOptions = null,
        ITypeAliasProvider? typeAliasProvider = null)
        : this(redisConfigurationOptions.Get(name), jsonSerializerOptions, typeAliasProvider)
    {
        redisConfigurationOptions.OnChange((option, optionName) =>
        {
            if (optionName == name)
            {
                InitRedisConfigurationOptions(option);
                GlobalCacheOptions = option.GlobalCacheOptions;
            }
        });
    }

    public RedisCacheClient(RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions = null,
        ITypeAliasProvider? typeAliasProvider = null)
        : base(redisConfigurationOptions, jsonSerializerOptions)
    {
        _typeAliasProvider = typeAliasProvider;
    }

    #region Get

    public override T? Get<T>(
        string key,
        Action<CacheOptions>? action = null) where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return GetAndRefresh<T>(FormatCacheKey<T>(key, action));
    }

    public override Task<T?> GetAsync<T>(
        string key,
        Action<CacheOptions>? action = null) where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return GetAndRefreshAsync<T>(FormatCacheKey<T>(key, action));
    }

    public override IEnumerable<T?> GetList<T>(
        IEnumerable<string> keys,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keys);

        var list = GetList(FormatCacheKeys<T>(keys, action), true);

        RefreshCore(list);

        return list.Select(option => ConvertToValue<T>(option.Value, out _)).ToList();
    }

    public override async Task<IEnumerable<T?>> GetListAsync<T>(
        IEnumerable<string> keys,
        Action<CacheOptions>? action = null)
        where T : default
    {
        ArgumentNullException.ThrowIfNull(keys);

        var list = await GetListAsync(FormatCacheKeys<T>(keys, action), true).ConfigureAwait(false);

        await RefreshCoreAsync(list).ConfigureAwait(false);

        return list.Select(option => ConvertToValue<T>(option.Value, out _)).ToList();
    }

    public override T? GetOrSet<T>(
        string key,
        Func<CacheEntry<T>> setter,
        Action<CacheOptions>? action = null) where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        ArgumentNullException.ThrowIfNull(setter);

        key = FormatCacheKey<T>(key, action);
        return GetAndRefresh(key, () =>
        {
            var cacheEntry = setter();
            if (cacheEntry.Value == null)
                return default;

            SetCore(key, cacheEntry.Value, cacheEntry);
            return cacheEntry.Value;
        });
    }

    public override async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<CacheEntry<T>>> setter,
        Action<CacheOptions>? action = null)
        where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        ArgumentNullException.ThrowIfNull(setter);

        key = FormatCacheKey<T>(key, action);
        return await GetAndRefreshAsync(key, async () =>
        {
            var cacheEntry = await setter();
            if (cacheEntry.Value == null)
                return default;

            await SetCoreAsync(key, cacheEntry.Value, cacheEntry).ConfigureAwait(false);
            return cacheEntry.Value;
        }).ConfigureAwait(false);
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

    public override IEnumerable<string> GetKeys<T>(
        string keyPattern,
        Action<CacheOptions>? action = null)
    {
        string formattedPattern = FormatKeyPattern<T>(keyPattern, action);
        return GetKeys(formattedPattern);
    }

    /// <summary>
    /// Only get the key, do not trigger the update expiration time policy
    /// </summary>
    /// <param name="keyPattern">keyPattern, such as: app_*</param>
    /// <returns></returns>
    public override async Task<IEnumerable<string>> GetKeysAsync(string keyPattern)
    {
        var prepared = LuaScript.Prepare(Const.GET_KEYS_SCRIPT);
        var cacheResult = await Db.ScriptEvaluateAsync(prepared, new { pattern = keyPattern }).ConfigureAwait(false);
        return (string[])cacheResult;
    }

    public override Task<IEnumerable<string>> GetKeysAsync<T>(
        string keyPattern,
        Action<CacheOptions>? action = null)
    {
        string formattedPattern = FormatKeyPattern<T>(keyPattern, action);

        return GetKeysAsync(formattedPattern);
    }

    public override IEnumerable<KeyValuePair<string, T?>> GetByKeyPattern<T>(
        string keyPattern,
        Action<CacheOptions>? action = null)
        where T : default
    {
        keyPattern = FormatKeyPattern<T>(keyPattern, action);

        var list = GetListByKeyPattern(keyPattern);

        RefreshCore(list);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value, out _)));
    }

    public override async Task<IEnumerable<KeyValuePair<string, T?>>> GetByKeyPatternAsync<T>(
        string keyPattern,
        Action<CacheOptions>? action = null) where T : default
    {
        keyPattern = FormatKeyPattern<T>(keyPattern, action);

        var list = await GetListByKeyPatternAsync(keyPattern).ConfigureAwait(false);

        await RefreshCoreAsync(list).ConfigureAwait(false);

        return list.Select(option => new KeyValuePair<string, T?>(option.Key, ConvertToValue<T>(option.Value, out _)));
    }

    #endregion

    #region Set

    public override void Set<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        SetCore(FormatCacheKey<T>(key, action), value, options);
    }

    private void SetCore<T>(string key, T value, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytesValue = value.ConvertFromValue(GlobalJsonSerializerOptions);

        Db.ScriptEvaluate(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue }));
    }

    public override Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return SetCoreAsync(FormatCacheKey<T>(key, action), value, options);
    }

    private Task SetCoreAsync<T>(string key, T value, CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytesValue = value.ConvertFromValue(GlobalJsonSerializerOptions);

        return Db.ScriptEvaluateAsync(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue })
        );
    }

    public override void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(GlobalJsonSerializerOptions)).ToArray();

        Db.ScriptEvaluate(
            Const.SET_MULTIPLE_SCRIPT,
            FormatCacheKeys<T>(keyValues.Select(item => item.Key), action).GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        );
    }

    public override async Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var redisValues = keyValues.Select(item => item.Value.ConvertFromValue(GlobalJsonSerializerOptions)).ToArray();

        await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_SCRIPT,
            FormatCacheKeys<T>(keyValues.Select(item => item.Key), action).GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options), () => redisValues)
        ).ConfigureAwait(false);
    }

    #endregion

    #region Refresh

    public override void Refresh(params string[] keys)
    {
        var list = GetList(keys, false);

        RefreshCore(list);
    }

    public override void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => Refresh(FormatCacheKeys<T>(keys, action).ToArray());

    public override async Task RefreshAsync(params string[] keys)
    {
        var list = await GetListAsync(keys, false).ConfigureAwait(false);

        await RefreshCoreAsync(list).ConfigureAwait(false);
    }

    public override Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => RefreshAsync(FormatCacheKeys<T>(keys, action).ToArray());

    #endregion

    #region Remove

    public override void Remove(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        Db.KeyDelete(keys.GetRedisKeys());
    }

    public override void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => Remove(FormatCacheKeys<T>(keys, action).ToArray());

    public override Task RemoveAsync(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        return Db.KeyDeleteAsync(keys.GetRedisKeys());
    }

    public override Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => RemoveAsync(FormatCacheKeys<T>(keys, action).ToArray());

    #endregion

    #region Exist

    public override bool Exists(string key)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return Db.KeyExists(key);
    }

    public override bool Exists<T>(string key, Action<CacheOptions>? action = null)
        => Exists(FormatCacheKey<T>(key, action));

    public override Task<bool> ExistsAsync(string key)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        return Db.KeyExistsAsync(key);
    }

    public override Task<bool> ExistsAsync<T>(string key, Action<CacheOptions>? action = null)
        => ExistsAsync(FormatCacheKey<T>(key, action));

    #endregion

    #region PubSub

    public override void Publish(string channel, Action<PublishOptions> options)
    {
        var publishOptions = GetAndCheckPublishOptions(channel, options);
        var message = JsonSerializer.Serialize(publishOptions, GlobalJsonSerializerOptions);
        Subscriber.Publish(channel, message);
    }

    public override async Task PublishAsync(string channel, Action<PublishOptions> options)
    {
        var publishOptions = GetAndCheckPublishOptions(channel, options);
        var message = JsonSerializer.Serialize(publishOptions, GlobalJsonSerializerOptions);
        await Subscriber.PublishAsync(channel, message).ConfigureAwait(false);
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

    public override void UnSubscribe<T>(string channel)
    {
        Subscriber.Unsubscribe(channel);
    }

    public override Task UnSubscribeAsync<T>(string channel)
    {
        return Subscriber.UnsubscribeAsync(channel);
    }

    #endregion

    #region Hash

    /// <summary>
    /// Increment Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">incremental increment, must be greater than 0</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty (is only initialized if the configuration does not exist)</param>
    /// <returns>Returns the field value after the increment operation</returns>
    public override async Task<long> HashIncrementAsync(
        string key,
        long value = 1,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null)
    {
        MasaArgumentException.ThrowIfLessThanOrEqual(value, 0L);

        var script = $@"
local exist = redis.call('EXISTS', KEYS[1])
if(exist ~= 1) then
redis.call('HMSET', KEYS[1], KEYS[3], ARGV[1], KEYS[4], ARGV[2])
    if ARGV[3] ~= '-1' then
        redis.call('EXPIRE', KEYS[1], ARGV[3])
    end
end
return redis.call('HINCRBY', KEYS[1], KEYS[2], {value})";

        var formattedKey = FormatCacheKey<long>(key, action);
        var result = (long)await Db.ScriptEvaluateAsync(script,
            new RedisKey[]
                { formattedKey, Const.DATA_KEY, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY },
            GetRedisValues(options)).ConfigureAwait(false);

        await RefreshAsync(formattedKey).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Descending Hash
    /// </summary>
    /// <param name="key">cache key</param>
    /// <param name="value">decrement increment, must be greater than 0</param>
    /// <param name="defaultMinVal">critical value</param>
    /// <param name="action">Cache configuration, used to change the global cache configuration information</param>
    /// <param name="options">Configure the cache life cycle, which is consistent with the default configuration when it is empty (is only initialized if the configuration does not exist)</param>
    /// <returns>Returns null on failure, and returns the field value after the decrement operation on success</returns>
    public override async Task<long?> HashDecrementAsync(
        string key,
        long value = 1L,
        long defaultMinVal = 0L,
        Action<CacheOptions>? action = null,
        CacheEntryOptions? options = null)
    {
        MasaArgumentException.ThrowIfLessThanOrEqual(value, 0L);

        var script = $@"
local exist = redis.call('EXISTS', KEYS[1])
if(exist ~= 1) then
redis.call('HMSET', KEYS[1], KEYS[3], ARGV[1], KEYS[4], ARGV[2])
    if ARGV[3] ~= '-1' then
        redis.call('EXPIRE', KEYS[1], ARGV[3])
    end
end

local result = redis.call('HGET', KEYS[1], KEYS[2])
if result then
else
    result = 0
end
if tonumber(result) > {defaultMinVal} then
    result = redis.call('HINCRBY', KEYS[1], KEYS[2], {0 - value})
    return result
else
    return nil
end";
        var formattedKey = FormatCacheKey<long>(key, action);
        var result = await Db.ScriptEvaluateAsync(
            script,
            new RedisKey[] { formattedKey, Const.DATA_KEY, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY },
            GetRedisValues(options)).ConfigureAwait(false);
        await RefreshAsync(formattedKey).ConfigureAwait(false);

        if (result.IsNull)
            return null;

        return (long)result;
    }

    #endregion

    #region Expire

    public override bool KeyExpire(string key, CacheEntryOptions? options = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        var result = Db.ScriptEvaluate(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        );

        return (long?)result == 1;
    }

    public override bool KeyExpire<T>(
        string key,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
        => KeyExpire(FormatCacheKey<T>(key, action), options);

    public override long KeyExpire(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = Db.ScriptEvaluate(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            keys.GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options))
        );

        return (long)result;
    }

    public override long KeyExpire<T>(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
        => KeyExpire(FormatCacheKeys<T>(keys, action), options);

    public override async Task<bool> KeyExpireAsync(
        string key,
        CacheEntryOptions? options = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_EXPIRATION_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options)
        ).ConfigureAwait(false);

        return (long)result == 1;
    }

    public override Task<bool> KeyExpireAsync<T>(
        string key,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
        => KeyExpireAsync(FormatCacheKey<T>(key, action), options);

    public override async Task<long> KeyExpireAsync(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var result = await Db.ScriptEvaluateAsync(
            Const.SET_MULTIPLE_EXPIRATION_SCRIPT,
            keys.GetRedisKeys(),
            GetRedisValues(GetCacheEntryOptions(options))
        ).ConfigureAwait(false);

        return (long)result;
    }

    public override Task<long> KeyExpireAsync<T>(
        IEnumerable<string> keys,
        CacheEntryOptions? options = null,
        Action<CacheOptions>? action = null)
        => KeyExpireAsync(FormatCacheKeys<T>(keys, action), options);

    #endregion

    #region Private methods

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
            Const.DATA_KEY).ConfigureAwait(false);

        var result = GetByArrayRedisValue<T>(results, key, out bool isExist);

        if (isExist)
            await RefreshAsync(result.model, flags).ConfigureAwait(false);
        else if (func != null)
            result.Value = await func.Invoke().ConfigureAwait(false);

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

    private string FormatCacheKey<T>(string key, Action<CacheOptions>? action)
        => CacheKeyHelper.FormatCacheKey<T>(key,
            GetCacheOptions(action).CacheKeyType!.Value,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName));

    private IEnumerable<string> FormatCacheKeys<T>(IEnumerable<string> keys, Action<CacheOptions>? action)
    {
        var cacheKeyType = GetCacheOptions(action).CacheKeyType!.Value;
        return keys.Select(key => CacheKeyHelper.FormatCacheKey<T>(
            key,
            cacheKeyType,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName)
        ));
    }

    private string FormatKeyPattern<T>(string keyPattern,
        Action<CacheOptions>? action = null)
    {
        return FormatCacheKey<T>(keyPattern, action).TrimEnd(keyPattern).Replace("[", "\\[").Replace("?", "\\?").Replace("*", "\\*") +
            keyPattern;
    }

    #endregion
}
