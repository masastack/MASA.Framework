// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public abstract class BaseDistributedCacheClient
{
    protected readonly IConnectionMultiplexer? Connection;
    protected readonly ISubscriber Subscriber;
    protected readonly IDatabase Db;
    protected readonly JsonSerializerOptions JsonSerializerOptions;
    protected readonly CacheEntryOptions? CacheEntryOptions;

    protected BaseDistributedCacheClient(
        ConfigurationOptions options,
        JsonSerializerOptions jsonSerializerOptions,
        CacheEntryOptions? cacheEntryOptions)
    {
        Connection = ConnectionMultiplexer.Connect(options);
        Db = Connection.GetDatabase();
        Subscriber = Connection.GetSubscriber();
        JsonSerializerOptions = jsonSerializerOptions;
        CacheEntryOptions = cacheEntryOptions;
    }

    protected T? ConvertToValue<T>(RedisValue value)
    {
        if (value.HasValue && !value.IsNullOrEmpty)
            return value.ConvertToValue<T>(JsonSerializerOptions);

        return default;
    }

    public static void CheckIsNullOrWhiteSpace(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(name);
    }

    public CacheEntryOptions GetCacheEntryOptions(CacheEntryOptions? options = null)
    {
        if (options != null)
            return options;

        if (CacheEntryOptions != null)
            return CacheEntryOptions;

        return CacheEntryOptions.Default;
    }

    protected void PublishCore(string channel, Action<PublishOptions> setup, Func<string, string, Task> func)
    {
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));

        ArgumentNullException.ThrowIfNull(setup, nameof(setup));

        var options = new PublishOptions();
        setup.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key))
            throw new ArgumentNullException(nameof(options.Key));

        var message = JsonSerializer.Serialize(options, JsonSerializerOptions);
        func.Invoke(channel, message);
    }

    internal async Task RefreshCoreAsync(
        List<RefreshCacheOptions> options,
        CancellationToken token = default)
    {
        var awaitRefreshOptions = GetKeyAndExpireList(options, token);
        if (awaitRefreshOptions.Count > 0)
        {
            string script = Const.SET_EXPIRE_SCRIPT.Replace("@data", GetSetExpireArrayOnLua(awaitRefreshOptions));
            await Db.ScriptEvaluateAsync(LuaScript.Prepare(script), new
            {
                length = awaitRefreshOptions.Count * 2
            });
        }
    }

    internal void RefreshCore(List<RefreshCacheOptions> options, CancellationToken token = default)
    {
        var awaitRefreshOptions = GetKeyAndExpireList(options, token);
        if (awaitRefreshOptions.Count > 0)
        {
            string script =
                Const.SET_EXPIRE_SCRIPT.Replace("@data", GetSetExpireArrayOnLua(awaitRefreshOptions));
            Db.ScriptEvaluate(LuaScript.Prepare(script), new
            {
                length = awaitRefreshOptions.Count * 2
            });
        }
    }

    internal List<(string Key, DateTimeOffset? AbsExpr, TimeSpan? SldExpr)> GetExpirationList(params string[] keys)
    {
        var arrayRedisResult = Db.ScriptEvaluate(
            LuaScript.Prepare(
                Const.GET_EXPIRATION_VALUE_SCRIPT.Replace("@keys", "{" + string.Join(',', keys.Select(key => $"'{key}'")) + "}")), new
            {
                absolute = Const.ABSOLUTE_EXPIRATION_KEY,
                sliding = Const.SLIDING_EXPIRATION_KEY
            });
        return GetExpirationListCore(arrayRedisResult.ToDictionary());
    }

    internal async Task<List<(string Key, DateTimeOffset? AbsExpr, TimeSpan? SldExpr)>> GetExpirationListAsync(params string[] keys)
    {
        var arrayRedisResult = await Db.ScriptEvaluateAsync(
            LuaScript.Prepare(
                Const.GET_EXPIRATION_VALUE_SCRIPT.Replace("@keys", "{" + string.Join(',', keys.Select(key => $"'{key}'")) + "}")), new
            {
                absolute = Const.ABSOLUTE_EXPIRATION_KEY,
                sliding = Const.SLIDING_EXPIRATION_KEY
            });
        return GetExpirationListCore(arrayRedisResult.ToDictionary());
    }

    private List<(string Key, DateTimeOffset? AbsExpr, TimeSpan? SldExpr)> GetExpirationListCore(
        Dictionary<string, RedisResult> arrayRedisResult)
    {
        List<(string Key, DateTimeOffset? AbsExpr, TimeSpan? SldExpr)> list = new();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            var options = MapMetadataByAutomatic(redisResult.Key, byteArray);
            list.Add((redisResult.Key, options.AbsExpr, options.SldExpr));
        }
        return list;
    }

    internal List<RefreshCacheOptions> GetListByKeyPattern(string keyPattern)
        => GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db.ScriptEvaluate(LuaScript.Prepare(script), parameters)
            .ToDictionary());

    internal Task<List<RefreshCacheOptions>> GetListByKeyPatternAsync(string keyPattern)
        => Task.FromResult(GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db
            .ScriptEvaluateAsync(LuaScript.Prepare(script), parameters)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
            .ToDictionary()));

    private List<RefreshCacheOptions> GetListCoreByKeyPattern(
        string keyPattern,
        Func<string, object, Dictionary<string, RedisResult>> func)
    {
        var arrayRedisResult = func.Invoke(
            Const.GET_KEY_AND_VALUE_SCRIPT,
            new
            {
                keypattern = keyPattern
            });

        List<RefreshCacheOptions> list = new List<RefreshCacheOptions>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(MapMetadataByAutomatic(redisResult.Key, byteArray));
        }
        return list;
    }

    internal List<RefreshCacheOptions> GetList(string[] keys)
        => GetListCore(keys,
            script => Db
                .ScriptEvaluate(LuaScript.Prepare(script))
                .ToDictionary());

    internal Task<List<RefreshCacheOptions>> GetListAsync(string[] keys)
        => Task.FromResult(GetListCore(keys,
            script => Db
                .ScriptEvaluateAsync(LuaScript.Prepare(script))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .ToDictionary()));

    private List<RefreshCacheOptions> GetListCore(string[] keys, Func<string, Dictionary<string, RedisResult>> func)
    {
        var arrayRedisResult =
            func.Invoke(Const.GET_LIST_SCRIPT.Replace("@keys", "{" + string.Join(',', keys.Select(key => $"'{key}'")) + "}"));

        List<RefreshCacheOptions> list = new List<RefreshCacheOptions>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(MapMetadataByAutomatic(redisResult.Key, byteArray));
        }
        return list;
    }

    private static List<KeyValuePair<string, TimeSpan?>> GetKeyAndExpireList(
        List<RefreshCacheOptions> options,
        CancellationToken token)
    {
        List<KeyValuePair<string, TimeSpan?>> list = new();

        var cacheEntryOptionsList = options.Select(opt => new KeyValuePair<string, CacheEntryOptions>(opt.Key, new CacheEntryOptions()
        {
            SlidingExpiration = opt.SldExpr,
            AbsoluteExpiration = opt.AbsExpr
        }));

        foreach (var item in cacheEntryOptionsList)
        {
            item.Value.RefreshCore(item.Key, (k, expr) =>
            {
                list.Add(new KeyValuePair<string, TimeSpan?>(k, expr));
                return Task.FromResult(false);
            }, token);
        }
        return list;
    }

    internal static RefreshCacheOptions MapMetadata(
        string key,
        RedisValue[] results)
    {
        DateTimeOffset? absoluteExpiration = null;
        TimeSpan? slidingExpiration = null;
        RedisValue data = results.Length > 2 ? results[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != Const.DEADLINE_LASTING)
            absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks.Value, TimeSpan.Zero);

        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != Const.DEADLINE_LASTING)
            slidingExpiration = new TimeSpan(slidingExpirationTicks.Value);
        return new RefreshCacheOptions(key, absoluteExpiration, slidingExpiration, data);
    }

    private static RefreshCacheOptions MapMetadataByAutomatic(string key, RedisValue[] results)
    {
        DateTimeOffset? absoluteExpiration = null;
        TimeSpan? slidingExpiration = null;
        RedisValue data = RedisValue.Null;

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
        return new RefreshCacheOptions(key, absoluteExpiration, slidingExpiration, data);
    }

    private static string GetSetExpireArrayOnLua(IEnumerable<KeyValuePair<string, TimeSpan?>> keyValuePairs)
        => "{" + string.Join(',', keyValuePairs.Select(kv => $"'{kv.Key}','{kv.Value?.TotalSeconds ?? -1}'")) + "}";
}
