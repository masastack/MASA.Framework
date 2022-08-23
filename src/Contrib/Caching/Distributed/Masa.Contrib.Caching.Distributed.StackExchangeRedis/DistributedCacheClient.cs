// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public class DistributedCacheClient : IDistributedCacheClient
{
    private readonly IConnectionMultiplexer? _connection;
    private readonly ISubscriber _subscriber;
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public DistributedCacheClient(ConfigurationOptions options, JsonSerializerOptions jsonSerializerOptions)
    {
        _connection = ConnectionMultiplexer.Connect(options);
        _db = _connection.GetDatabase();
        _subscriber = _connection.GetSubscriber();
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public T? Get<T>(string key)
    {
        var redisValue = GetAndRefresh(key, getData: true);
        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
            return redisValue.ConvertToValue<T>(_jsonSerializerOptions);

        return default;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var redisValue = await GetAndRefreshAsync(key, getData: true);
        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
            return redisValue.ConvertToValue<T>(_jsonSerializerOptions);

        return default;
    }

    public T? Get<T>(string key, Action<T?> valueChanged)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string key, Action<T?> valueChanged)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<T?> GetList<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        throw new NotImplementedException();
    }

    public async Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));
        return await GetAndRefreshAsync<T>(keys);
    }

    public T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter)
    {
        throw new NotImplementedException();
    }

    public void Set<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public void Set<T>(string key, T value, CacheEntryOptions<T>? options = null)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        var bytesValue = value.ConvertFromValue(_jsonSerializerOptions);
        await _db.ScriptEvaluateAsync(
            Const.SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options, () => new[] { bytesValue })
        ).ConfigureAwait(false);
    }

    public void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null)
    {
        throw new NotImplementedException();
    }

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null)
    {
        throw new NotImplementedException();
    }

    public void Refresh(string key)
    {
        throw new NotImplementedException();
    }

    public Task RefreshAsync(string key)
    {
        throw new NotImplementedException();
    }

    public void Remove(params string[] keys)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(params string[] keys)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }

    public List<string> GetKeys(string keyPattern)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetKeysAsync(string keyPattern)
    {
        throw new NotImplementedException();
    }

    public List<KeyValuePair<string, T?>> GetListByKeyPattern<T>(string keyPattern)
    {
        throw new NotImplementedException();
    }

    public Task<List<KeyValuePair<string, T?>>> GetListByKeyPatternAsync<T>(string keyPattern)
    {
        throw new NotImplementedException();
    }

    public void Publish(string channel, Action<PublishOptions> setup)
    {
        throw new NotImplementedException();
    }

    public Task PublishAsync(string channel, Action<PublishOptions> setup)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        throw new NotImplementedException();
    }

    public Task<long> HashIncrementAsync(string key, long value = 1)
    {
        throw new NotImplementedException();
    }

    public Task<long> HashDecrementAsync(string key, long value = 1)
    {
        throw new NotImplementedException();
    }

    public bool KeyExpire(string key, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public bool KeyExpire(string key, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExpireAsync(string key, TimeSpan absoluteExpirationRelativeToNow)
    {
        throw new NotImplementedException();
    }

    public bool KeyExpire(string key, CacheEntryOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null)
    {
        throw new NotImplementedException();
    }

    private RedisValue[] GetRedisValues(CacheEntryOptions? cacheEntryOptions, Func<RedisValue[]>? func = null)
    {
        var creationTime = DateTimeOffset.UtcNow;
        var absoluteExpiration = GetAbsoluteExpiration(creationTime, cacheEntryOptions);
        List<RedisValue> redisValues = new()
        {
            absoluteExpiration?.Ticks ?? Const.DEADLINE_LASTING,
            cacheEntryOptions?.SlidingExpiration?.Ticks ?? Const.DEADLINE_LASTING,
            GetExpirationInSeconds(creationTime, absoluteExpiration, cacheEntryOptions) ?? Const.DEADLINE_LASTING,
        };
        if (func != null)
        {
            redisValues.AddRange(func.Invoke());
        }
        return redisValues.ToArray();
    }

    private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, CacheEntryOptions? options)
    {
        if (options == null)
            return null;

        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            throw new ArgumentOutOfRangeException(
                nameof(CacheEntryOptions.AbsoluteExpiration),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            return creationTime.Add(options.AbsoluteExpirationRelativeToNow.Value);

        return options.AbsoluteExpiration;
    }

    private static void MapMetadataByAutomatic(RedisValue[] results, out DateTimeOffset? absoluteExpiration,
        out TimeSpan? slidingExpiration,
        out RedisValue data)
    {
        absoluteExpiration = null;
        slidingExpiration = null;
        data = RedisValue.Null;

        for (int index = 0; index < results.Length; index += 2)
        {
            if (results[index] == Const.ABSOLUTE_EXPIRATION_KEY)
            {
                var absoluteExpirationTicks = (long?)results[index + 1];
                if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != Const.DEADLINE_LASTING)
                {
                    absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);
                }
            }
            else if (results[index] == Const.SLIDING_EXPIRATION_KEY)
            {
                var slidingExpirationTicks = (long?)results[index + 1];
                if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != Const.DEADLINE_LASTING)
                {
                    slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
                }
            }
            else if (results[index] == Const.DATA_KEY)
            {
                data = results[index + 1];
            }
        }
    }

    private static long? GetExpirationInSeconds(
        DateTimeOffset creationTime,
        DateTimeOffset? absoluteExpiration,
        CacheEntryOptions? options)
    {
        if (options == null)
            return null;

        if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
            return (long)Math.Min(
                (absoluteExpiration.Value - creationTime).TotalSeconds,
                options.SlidingExpiration.Value.TotalSeconds);

        if (absoluteExpiration.HasValue)
            return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;

        if (options is { SlidingExpiration: { } })
            return (long)options.SlidingExpiration.Value.TotalSeconds;

        return null;
    }

    private RedisValue GetAndRefresh(string key, bool getData, CommandFlags flags = CommandFlags.None)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var results = getData ? _db.HashMemberGet(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY, Const.DATA_KEY) :
            _db.HashMemberGet(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY);

        MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr, out RedisValue data);
        Refresh(key, absExpr, sldExpr, flags);

        return data;
    }

    private Task<List<T?>> GetAndRefreshAsync<T>(string[] keys, CommandFlags flags = CommandFlags.None)
    {
        ArgumentNullException.ThrowIfNull(keys, nameof(keys));

        string keysStr = "{" + string.Join(',', keys.Select(key => $"'{key}'")) + "}";

        var arrayRedisResult = _db
            .ScriptEvaluate(LuaScript.Prepare(Const.GET_LIST_SCRIPT.Replace("@keys", keysStr)))
            .ToDictionary();

        var dictionary =
            GetListByArrayRedisResultCore<T>(arrayRedisResult,
                async (key, absExpr, sldExpr) => await RefreshAsync(key, absExpr, sldExpr, flags));
        return Task.FromResult(dictionary.Select(d => d.Value).ToList());
    }

    private Dictionary<string, T?> GetListByArrayRedisResultCore<T>(
        Dictionary<string, RedisResult> arrayRedisResult,
        Func<string, DateTimeOffset?, TimeSpan?, Task> func)
    {
        Dictionary<string, T?> dictionary = new();

        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            MapMetadataByAutomatic(byteArray, out DateTimeOffset? absExpr, out TimeSpan? sldExpr, out RedisValue data);
            func.Invoke(redisResult.Key, absExpr, sldExpr);
            dictionary.Add(redisResult.Key, data.ConvertToValue<T>(_jsonSerializerOptions));
        }

        return dictionary;
    }

    private async Task<RedisValue> GetAndRefreshAsync(string key, bool getData, CommandFlags flags = CommandFlags.None)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var results = getData ?
            await _db.HashMemberGetAsync(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY, Const.DATA_KEY)
                .ConfigureAwait(false) :
            await _db.HashMemberGetAsync(key, Const.ABSOLUTE_EXPIRATION_KEY, Const.SLIDING_EXPIRATION_KEY).ConfigureAwait(false);

        MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr, out var data);
        await RefreshAsync(key, absExpr, sldExpr, flags).ConfigureAwait(false);
        return data;
    }

    private void Refresh(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, CommandFlags flags)
    {
        RefreshCore(key, absExpr, sldExpr, (k, expr) =>
        {
            _db.KeyExpire(k, expr, flags);
            return Task.CompletedTask;
        });
    }

    private async Task RefreshAsync(
        string key,
        DateTimeOffset? absExpr,
        TimeSpan? sldExpr,
        CommandFlags flags,
        CancellationToken token = default)
    {
        RefreshCore(key, absExpr, sldExpr, async (k, expr) =>
        {
            await _db.KeyExpireAsync(k, expr, flags).ConfigureAwait(false);
        }, token);
        await Task.CompletedTask;
    }

    private void RefreshCore(
        string key,
        DateTimeOffset? absExpr,
        TimeSpan? sldExpr,
        Func<string, TimeSpan?, Task> func,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        token.ThrowIfCancellationRequested();

        // Note Refresh has no effect if there is just an absolute expiration (or neither).
        if (sldExpr.HasValue)
        {
            TimeSpan? expr;
            if (absExpr.HasValue)
            {
                var relExpr = absExpr.Value - DateTimeOffset.Now;
                expr = relExpr <= sldExpr.Value ? relExpr : sldExpr;
            }
            else
            {
                expr = sldExpr;
            }

            func.Invoke(key, expr);
        }
    }

    private static void MapMetadata(RedisValue[] results,
        out DateTimeOffset? absoluteExpiration,
        out TimeSpan? slidingExpiration,
        out RedisValue data)
    {
        absoluteExpiration = null;
        slidingExpiration = null;
        data = results.Length > 2 ? results[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != Const.DEADLINE_LASTING)
            absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);

        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != Const.DEADLINE_LASTING)
            slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
    }
}
