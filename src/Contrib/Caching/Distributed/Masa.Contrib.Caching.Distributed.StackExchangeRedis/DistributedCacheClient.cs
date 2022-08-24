// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// todo: waiting for optimization
/// </summary>
public class DistributedCacheClient : BaseDistributedCacheClient, IDistributedCacheClient
{
    private readonly SubscribeConfigurationOptions _subscribeConfigurationOptions;
    private readonly object _locker = new();
    private readonly IList<string> _subscribeChannels = new List<string>();

    public DistributedCacheClient(
        ConfigurationOptions options,
        JsonSerializerOptions jsonSerializerOptions,
        SubscribeConfigurationOptions? subscribeConfigurationOptions = null,
        CacheEntryOptions? cacheEntryOptions = null)
        : base(options, jsonSerializerOptions, cacheEntryOptions)
    {
        _subscribeConfigurationOptions = subscribeConfigurationOptions ?? SubscribeConfigurationOptions.Default;
    }

    public T? Get<T>(string key)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var redisValue = GetAndRefresh(key, getData: true);
        return ConvertToValue<T>(redisValue);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var redisValue = await GetAndRefreshAsync(key, getData: true);
        return ConvertToValue<T>(redisValue);
    }

    public T? Get<T>(string key, Action<T?> valueChanged)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        var value = Get<T>(key);

        if (value != null)
        {
            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new CombinedCacheEntryOptions<T>
            {
                ValueChanged = valueChanged
            });
        }
        return value;
    }

    public async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));
        var value = await GetAsync<T>(key);

        if (value != null)
        {
            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new CombinedCacheEntryOptions<T>
            {
                ValueChanged = valueChanged
            });
        }
        return value;
    }

    public IEnumerable<T?> GetList<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        return GetAndRefresh<T>(keys);
    }

    public async Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        return await GetAndRefreshAsync<T>(keys);
    }

    public T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        var redisValue = GetAndRefresh(key, true);

        T? value = ConvertToValue<T>(redisValue);
        if (value == null)
        {
            var cacheEntry = setter();

            Set(key, cacheEntry.Value, cacheEntry);
        }

        return value;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter)
    {
        CheckIsNullOrWhiteSpace(key, nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        var redisValue = await GetAndRefreshAsync(key, true);

        T? value = ConvertToValue<T>(redisValue);
        if (value == null)
        {
            var cacheEntry = setter();

            await SetAsync(key, cacheEntry.Value, cacheEntry);
        }

        return value;
    }

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

    public void Refresh(params string[] keys)
    {
        var expirationList = GetExpirationList(keys);

        var options = expirationList
            .Select(opt => new RefreshCacheOptions(opt.Key, opt.AbsExpr, opt.SldExpr, RedisValue.Null))
            .ToList();

        RefreshCore(options);
    }

    public async Task RefreshAsync(params string[] keys)
    {
        var expirationList = await GetExpirationListAsync(keys);

        var options = expirationList
            .Select(opt => new RefreshCacheOptions(opt.Key, opt.AbsExpr, opt.SldExpr, RedisValue.Null))
            .ToList();

        await RefreshCoreAsync(options);
    }

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

    public Task<long> HashIncrementAsync(string key, long value = 1)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        return Db.HashIncrementAsync(key, Const.DATA_KEY, value);
    }

    public async Task<long> HashDecrementAsync(string key, long value = 1)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 0");

        var script = $@"
local result = redis.call('HGET', KEYS[1], KEYS[2])
if tonumber(result) >= {value} then
    result = redis.call('HINCRBY', KEYS[1], KEYS[2], {0 - value})
    return result
else
    return -1
end";
        var result = (long)await Db.ScriptEvaluateAsync(script, new RedisKey[] { key, Const.DATA_KEY });

        return result;
    }

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
        var cacheEntryOptions = GetCacheEntryOptions(options);
        if (cacheEntryOptions.SlidingExpiration == null)
        {
            var dateTimeOffset = cacheEntryOptions.GetAbsoluteExpiration(DateTimeOffset.Now);
            Db.KeyExpire(key, dateTimeOffset?.Offset);
        }
        return cacheEntryOptions.RefreshCore(key, async (k, expr) => await Db.KeyExpireAsync(k, expr).ConfigureAwait(false));
    }

    public Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
    {
        var cacheEntryOptions = GetCacheEntryOptions(options);
        if (cacheEntryOptions.SlidingExpiration == null)
        {
            var dateTimeOffset = cacheEntryOptions.GetAbsoluteExpiration(DateTimeOffset.Now);
            Db.KeyExpire(key, dateTimeOffset?.Offset);
        }
        return Task.FromResult(
            cacheEntryOptions.RefreshCore(key, async (k, expr) => await Db.KeyExpireAsync(k, expr).ConfigureAwait(false)));
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
            TimeHelper.GetExpirationInSeconds(creationTime, absoluteExpiration, cacheEntryOptions) ?? Const.DEADLINE_LASTING,
        };
        if (func != null)
            redisValues.AddRange(func.Invoke());

        return redisValues.ToArray();
    }

    private RedisValue GetAndRefresh(string key, bool getData, CommandFlags flags = CommandFlags.None)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var results = getData ? Db.HashMemberGet(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY, Const.DATA_KEY) :
            Db.HashMemberGet(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY);

        var options = MapMetadata(key, results);
        Refresh(options, flags);
        return options.Value;
    }

    private List<T?> GetAndRefresh<T>(string[] keys, CommandFlags flags = CommandFlags.None)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var list = GetList(keys);

        RefreshCore(list);

        return list.Select(option => ConvertToValue<T>(option.Value)).ToList();
    }

    private async Task<List<T?>> GetAndRefreshAsync<T>(string[] keys, CommandFlags flags = CommandFlags.None)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        var list = await GetListAsync(keys);

        await RefreshCoreAsync(list);

        return list.Select(option => ConvertToValue<T>(option.Value)).ToList();
    }

    private async Task<RedisValue> GetAndRefreshAsync(string key, bool getData, CommandFlags flags = CommandFlags.None)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var results = getData ?
            await Db.HashMemberGetAsync(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY, Const.DATA_KEY)
                .ConfigureAwait(false) :
            await Db.HashMemberGetAsync(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY).ConfigureAwait(false);

        var options = MapMetadata(key, results);
        await RefreshAsync(options, flags).ConfigureAwait(false);
        return options.Value;
    }

    private void Refresh(RefreshCacheOptions refreshCacheOptions, CommandFlags flags)
    {
        new CacheEntryOptions
        {
            AbsoluteExpiration = refreshCacheOptions.AbsExpr,
            SlidingExpiration = refreshCacheOptions.SldExpr,
        }.RefreshCore(refreshCacheOptions.Key, (k, expr) => Task.FromResult(Db.KeyExpire(k, expr, flags)));
    }

    private async Task RefreshAsync(
        RefreshCacheOptions refreshCacheOptions,
        CommandFlags flags,
        CancellationToken token = default)
    {
        new CacheEntryOptions()
        {
            AbsoluteExpiration = refreshCacheOptions.AbsExpr,
            SlidingExpiration = refreshCacheOptions.SldExpr,
        }.RefreshCore(refreshCacheOptions.Key, async (k, expr)
            => await Db.KeyExpireAsync(k, expr, flags).ConfigureAwait(false), token);
        await Task.CompletedTask;
    }

    private string FormatSubscribeChannel<T>(string key) =>
        SubscribeHelper.FormatSubscribeChannel<T>(key,
            _subscribeConfigurationOptions.SubscribeKeyTypes,
            _subscribeConfigurationOptions.SubscribeKeyPrefix);

    private void Subscribe<T>(string channel, CombinedCacheEntryOptions<T>? options = null)
    {
        lock (_locker)
        {
            if (!_subscribeChannels.Contains(channel))
            {
                lock (_locker)
                {
                    if (!_subscribeChannels.Contains(channel))
                    {
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
            }
        }
    }
}
