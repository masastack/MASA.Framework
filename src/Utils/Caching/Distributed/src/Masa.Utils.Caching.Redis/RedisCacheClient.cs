// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Redis;

/// <summary>
/// The redis cache client.
/// </summary>
public class RedisCacheClient : IDistributedCacheClient
{
    // Reference from https://github.com/dotnet/aspnetcore/blob/3c793666742cfc4c389292f3378d15e32f860dc9/src/Caching/StackExchangeRedis/src/RedisCache.cs#L372
    // KEYS[1] = = key
    // ARGV[1] = absolute-expiration - ticks as long (-1 for none)
    // ARGV[2] = sliding-expiration - ticks as long (-1 for none)
    // ARGV[3] = relative-expiration (long, in seconds, -1 for none) - Min(absolute-expiration - Now, sliding-expiration)
    // ARGV[4] = data - byte[]
    // this order should not change LUA script depends on it
    private const string SET_SCRIPT = @"
                redis.call('HSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1";

    private const string SET_MULTIPLE_SCRIPT = @"
                local count = 0
                for i, key in ipairs(KEYS) do
                  redis.call('HSET', key, 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[i+3])
                  if ARGV[3] ~= '-1' then
                    redis.call('EXPIRE', key, ARGV[3])
                  end
                  count = count + 1
                end
                return count";

    private const string ABSOLUTE_EXPIRATION_KEY = "absexp";
    private const string SLIDING_EXPIRATION_KEY = "sldexp";
    private const string DATA_KEY = "data";
    private const long NOT_PRESENT = -1;

    internal static readonly RedisConfigurationOptions RedisConfiguration = new();

    private readonly IConnectionMultiplexer? _connection;
    private readonly IDatabase _db;
    private readonly ISubscriber _subscriber;

    public RedisCacheClient(ConfigurationOptions options)
    {
        _connection = ConnectionMultiplexer.Connect(options);
        _db = _connection.GetDatabase();
        _subscriber = _connection.GetSubscriber();
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        var redisValue = GetAndRefresh(key, getData: true);
        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
        {
            return ConvertToValue<T>(redisValue);
        }

        return default;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        var redisValue = await GetAndRefreshAsync(key, getData: true);
        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
        {
            return ConvertToValue<T>(redisValue);
        }

        return default;
    }

    /// <inheritdoc />
    public T? GetOrSet<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T?>? options = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (setter == null)
            throw new ArgumentNullException(nameof(setter));

        T? value;

        var redisValue = GetAndRefresh(key, true);

        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
        {
            value = ConvertToValue<T>(redisValue);
        }
        else
        {
            value = setter();

            Set(key, value, options);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T>? options = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (setter == null)
            throw new ArgumentNullException(nameof(setter));

        T? value;

        var redisValue = await GetAndRefreshAsync(key, getData: true);

        if (redisValue.HasValue && !redisValue.IsNullOrEmpty)
        {
            value = ConvertToValue<T>(redisValue);
        }
        else
        {
            value = setter();

            await SetAsync(key, value, options);
        }

        return value;
    }

    /// <inheritdoc />
    public IEnumerable<T?> GetList<T>(string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        // TODO: whether need to check keys.length

        var redisValues = keys.Select(key => GetAndRefresh(key, getData: true));

        return redisValues
            .Where(v => v.HasValue && !v.IsNullOrEmpty)
            .Select(ConvertToValue<T>);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T?>> GetListAsync<T>(string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        // TODO: whether need to check keys.length

        var redisValues = await Task.WhenAll(keys.Select(key => GetAndRefreshAsync(key, getData: true)));

        return redisValues
            .Where(v => v.HasValue && !v.IsNullOrEmpty)
            .Select(ConvertToValue<T>);
    }

    /// <inheritdoc />
    public void Remove<T>(params string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        _db.KeyDelete(keys.Select(key => (RedisKey)key).ToArray());
    }

    /// <inheritdoc />
    public async Task RemoveAsync<T>(params string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        await _db.KeyDeleteAsync(keys.Select(key => (RedisKey)key).ToArray());
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (options == null)
            options = new CombinedCacheEntryOptions<T>();

        var distributedCacheEntryOptions = options.DistributedCacheEntryOptions;

        var bytesValue = ConvertFromValue(value);

        var creationTime = DateTimeOffset.UtcNow;
        var absoluteExpiration = GetAbsoluteExpiration(creationTime, distributedCacheEntryOptions);

        var redisKeys = new RedisKey[] { key };
        var redisValues = new RedisValue[]
        {
            absoluteExpiration?.Ticks ?? NOT_PRESENT,
            distributedCacheEntryOptions?.SlidingExpiration?.Ticks ?? NOT_PRESENT,
            GetExpirationInSeconds(creationTime, absoluteExpiration, distributedCacheEntryOptions) ?? NOT_PRESENT,
            bytesValue
        };

        _db.ScriptEvaluate(SET_SCRIPT, redisKeys, redisValues);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        ArgumentNullException.ThrowIfNull(value, nameof(value));

        options ??= new CombinedCacheEntryOptions<T>();

        var bytesValue = ConvertFromValue(value);

        await _db.ScriptEvaluateAsync(
            SET_SCRIPT,
            new RedisKey[] { key },
            GetRedisValues(options.DistributedCacheEntryOptions, () => new[] { (RedisValue)bytesValue })
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(keyValues, nameof(keyValues));

        options ??= new CombinedCacheEntryOptions<T>();

        var redisKeys = keyValues.Select(item => (RedisKey)item.Key).ToArray();
        var redisValues = keyValues.Select(item => ConvertFromValue(item.Value)).ToArray();

        _db.ScriptEvaluate(
            SET_MULTIPLE_SCRIPT,
            redisKeys,
            GetRedisValues(options.DistributedCacheEntryOptions, () => redisValues)
        );
    }

    /// <inheritdoc />
    public async Task SetListAsync<T>(Dictionary<string, T> keyValues, CombinedCacheEntryOptions<T>? options = null)
    {
        if (keyValues == null)
            throw new ArgumentNullException(nameof(keyValues));

        options ??= new CombinedCacheEntryOptions<T>();

        var keys = keyValues.Select(item => (RedisKey)item.Key).ToArray();
        var redisValues = keyValues.Select(item => ConvertFromValue(item.Value)).ToArray();

        await _db.ScriptEvaluateAsync(
            SET_MULTIPLE_SCRIPT,
            keys,
            GetRedisValues(options.DistributedCacheEntryOptions, () => redisValues)
        ).ConfigureAwait(false);
    }

    private RedisValue[] GetRedisValues(DistributedCacheEntryOptions? distributedCacheEntryOptions, Func<RedisValue[]>? func = null)
    {
        var creationTime = DateTimeOffset.UtcNow;
        var absoluteExpiration = GetAbsoluteExpiration(creationTime, distributedCacheEntryOptions);
        List<RedisValue> redisValues = new()
        {
            absoluteExpiration?.Ticks ?? NOT_PRESENT,
            distributedCacheEntryOptions?.SlidingExpiration?.Ticks ?? NOT_PRESENT,
            GetExpirationInSeconds(creationTime, absoluteExpiration, distributedCacheEntryOptions) ?? NOT_PRESENT,
        };
        if (func != null)
        {
            redisValues.AddRange(func.Invoke());
        }
        return redisValues.ToArray();
    }

    /// <inheritdoc />
    public bool Exists<T>(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return _db.KeyExists(key);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync<T>(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return await _db.KeyExistsAsync(key);
    }

    /// <inheritdoc />
    public void Refresh(string key)
    {
        GetAndRefresh(key, getData: false);
    }

    /// <inheritdoc />
    public async Task RefreshAsync(string key)
    {
        await GetAndRefreshAsync(key, getData: false);
    }

    /// <inheritdoc />
    public void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        _subscriber.Subscribe(channel, (_, message) =>
        {
            var options = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            handler(options!);
        });
    }

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler)
    {
        await _subscriber.SubscribeAsync(channel, (_, message) =>
        {
            var options = JsonSerializer.Deserialize<SubscribeOptions<T>>(message);
            handler(options!);
        });
    }

    /// <inheritdoc />
    public void Publish<T>(string channel, Action<SubscribeOptions<T>> setup)
    {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        if (setup == null)
            throw new ArgumentNullException(nameof(setup));

        var options = new SubscribeOptions<T>();
        setup.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key))
            throw new ArgumentNullException(nameof(options.Key));

        var message = JsonSerializer.Serialize(options);

        _subscriber.Publish(channel, message);
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(string channel, Action<SubscribeOptions<T>> setup)
    {
        if (channel == null)
            throw new ArgumentNullException(nameof(channel));

        if (setup == null)
            throw new ArgumentNullException(nameof(setup));

        var options = new SubscribeOptions<T>();
        setup.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key))
            throw new ArgumentNullException(nameof(options.Key));

        var message = JsonSerializer.Serialize(options);

        await _subscriber.PublishAsync(channel, message);
    }

    public async Task<long> HashIncrementAsync(string key, long value = 1L)
    {
        return await _db.HashIncrementAsync(key, DATA_KEY, value);
    }

    public async Task<long> HashDecrementAsync(string key, long value = 1L)
    {
        var script = $@"
local result = redis.call('HGET', KEYS[1], KEYS[2])
if tonumber(result) >= {value} then
    result = redis.call('HINCRBY', KEYS[1], KEYS[2], {0 - value})
    return result
else
    return -1
end";
        var result = (long)await _db.ScriptEvaluateAsync(script, new RedisKey[] { key, DATA_KEY });

        return result;
    }

    private RedisValue GetAndRefresh(string key, bool getData)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        RedisValue[] results;
        if (getData)
        {
            results = _db.HashMemberGet(key, ABSOLUTE_EXPIRATION_KEY, SLIDING_EXPIRATION_KEY, DATA_KEY);
        }
        else
        {
            results = _db.HashMemberGet(key, ABSOLUTE_EXPIRATION_KEY, SLIDING_EXPIRATION_KEY);
        }

        if (results.Length >= 2)
        {
            MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
            Refresh(key, absExpr, sldExpr);
        }

        if (results.Length >= 3 && results[2].HasValue)
        {
            return results[2];
        }

        return RedisValue.Null;
    }

    private async Task<RedisValue> GetAndRefreshAsync(string key, bool getData)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        RedisValue[] results;
        if (getData)
        {
            results = await _db.HashMemberGetAsync(key, ABSOLUTE_EXPIRATION_KEY, SLIDING_EXPIRATION_KEY, DATA_KEY).ConfigureAwait(false);
        }
        else
        {
            results = await _db.HashMemberGetAsync(key, ABSOLUTE_EXPIRATION_KEY, SLIDING_EXPIRATION_KEY).ConfigureAwait(false);
        }

        // TODO: Error handling
        if (results.Length >= 2)
        {
            MapMetadata(results, out DateTimeOffset? absExpr, out TimeSpan? sldExpr);
            await RefreshAsync(key, absExpr, sldExpr).ConfigureAwait(false);
        }

        if (results.Length >= 3 && results[2].HasValue)
        {
            return results[2];
        }

        return RedisValue.Null;
    }

    private void Refresh(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

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

            _db.KeyExpire(key, expr);
            // TODO: Error handling
        }
    }

    private async Task RefreshAsync(string key, DateTimeOffset? absExpr, TimeSpan? sldExpr, CancellationToken token = default)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

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

            await _db.KeyExpireAsync(key, expr).ConfigureAwait(false);
            // TODO: Error handling
        }
    }

    private static void MapMetadata(RedisValue[] results, out DateTimeOffset? absoluteExpiration, out TimeSpan? slidingExpiration)
    {
        absoluteExpiration = null;
        slidingExpiration = null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != NOT_PRESENT)
        {
            absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);
        }

        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != NOT_PRESENT)
        {
            slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
        }
    }

    private static long? GetExpirationInSeconds(
        DateTimeOffset creationTime,
        DateTimeOffset? absoluteExpiration,
        DistributedCacheEntryOptions? options)
    {
        if (options == null)
            return null;

        if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
        {
            return (long)Math.Min(
                (absoluteExpiration.Value - creationTime).TotalSeconds,
                options.SlidingExpiration.Value.TotalSeconds);
        }

        if (absoluteExpiration.HasValue)
        {
            return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
        }

        if (options is { SlidingExpiration: { } })
        {
            return (long)options.SlidingExpiration.Value.TotalSeconds;
        }

        return null;
    }

    private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions? options)
    {
        if (options == null)
            return null;

        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
        {
            throw new ArgumentOutOfRangeException(
                nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");
        }

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            return creationTime + options.AbsoluteExpirationRelativeToNow;
        }

        return options.AbsoluteExpiration;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_connection != null)
            _connection.Dispose();
    }
}
