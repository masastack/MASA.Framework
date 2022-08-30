// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public abstract class BaseDistributedCacheClient
{
    protected static Guid UniquelyIdentifies;
    protected ISubscriber Subscriber;
    protected IDatabase Db;
    protected readonly JsonSerializerOptions JsonSerializerOptions;
    protected readonly CacheEntryOptions CacheEntryOptions;

    static BaseDistributedCacheClient() => UniquelyIdentifies = Guid.NewGuid();

    protected BaseDistributedCacheClient(RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions)
    {
        var redisConfiguration = GetRedisConfigurationOptions(redisConfigurationOptions);
        IConnectionMultiplexer? connection = ConnectionMultiplexer.Connect(redisConfiguration);
        Db = connection.GetDatabase();
        Subscriber = connection.GetSubscriber();

        JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions().EnableDynamicTypes();

        CacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpiration = redisConfiguration.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = redisConfiguration.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = redisConfiguration.SlidingExpiration
        };
    }

    private RedisConfigurationOptions GetRedisConfigurationOptions(RedisConfigurationOptions redisConfigurationOptions)
    {
        if (redisConfigurationOptions.Servers.Any())
            return redisConfigurationOptions;

        return new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            }
        };
    }

    protected T? ConvertToValue<T>(RedisValue value)
    {
        if (value.HasValue && !value.IsNullOrEmpty)
            return value.ConvertToValue<T>(JsonSerializerOptions);

        return default;
    }

    protected CacheEntryOptions GetCacheEntryOptions(CacheEntryOptions? options = null)
        => options ?? CacheEntryOptions;

    protected void PublishCore(string channel, Action<PublishOptions> setup, Func<string, string, Task> func)
    {
        ArgumentNullException.ThrowIfNull(channel, nameof(channel));

        ArgumentNullException.ThrowIfNull(setup, nameof(setup));

        var options = new PublishOptions(UniquelyIdentifies);
        setup.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key))
            throw new ArgumentNullException(nameof(options.Key));

        var message = JsonSerializer.Serialize(options, JsonSerializerOptions);
        func.Invoke(channel, message);
    }

    internal void RefreshCore(List<DataCacheOptions> options, CancellationToken token = default)
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

    internal async Task RefreshCoreAsync(
        List<DataCacheOptions> options,
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

    internal List<DataCacheOptions> GetListByKeyPattern(string keyPattern)
        => GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db.ScriptEvaluate(LuaScript.Prepare(script), parameters)
            .ToDictionary());

    internal Task<List<DataCacheOptions>> GetListByKeyPatternAsync(string keyPattern)
        => Task.FromResult(GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db
            .ScriptEvaluateAsync(LuaScript.Prepare(script), parameters)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
            .ToDictionary()));

    private List<DataCacheOptions> GetListCoreByKeyPattern(
        string keyPattern,
        Func<string, object, Dictionary<string, RedisResult>> func)
    {
        var arrayRedisResult = func.Invoke(
            Const.GET_KEY_AND_VALUE_SCRIPT,
            new
            {
                keypattern = keyPattern
            });

        List<DataCacheOptions> list = new List<DataCacheOptions>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(MapMetadataByAutomatic(redisResult.Key, byteArray));
        }
        return list;
    }

    internal List<DataCacheOptions> GetList(string[] keys, bool getData)
    {
        string script = getData ? Const.GET_LIST_SCRIPT : Const.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = Db
            .ScriptEvaluate(script, keys.Select(key => (RedisKey)key).ToArray())
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult);
    }

    internal async Task<List<DataCacheOptions>> GetListAsync(string[] keys, bool getData)
    {
        string script = getData ? Const.GET_LIST_SCRIPT : Const.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = (await Db
                .ScriptEvaluateAsync(script, keys.Select(key => (RedisKey)key).ToArray()))
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult);
    }

    private List<DataCacheOptions> GetListByArrayRedisResult(Dictionary<string, RedisResult> arrayRedisResult)
    {
        List<DataCacheOptions> list = new List<DataCacheOptions>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(MapMetadataByAutomatic(redisResult.Key, byteArray));
        }
        return list;
    }

    private static List<KeyValuePair<string, TimeSpan?>> GetKeyAndExpireList(
        List<DataCacheOptions> options,
        CancellationToken token)
    {
        List<KeyValuePair<string, TimeSpan?>> list = new();

        DateTimeOffset? creationTime = DateTimeOffset.UtcNow;
        foreach (var option in options)
        {
            var res = option.GetExpiration(creationTime, token);
            if (res.State)
            {
                list.Add(new KeyValuePair<string, TimeSpan?>(option.Key, res.Expire));
            }
        }
        return list;
    }

    internal static DataCacheOptions MapMetadata(
        string key,
        RedisValue[] results)
    {
        RedisValue data = results.Length > 2 ? results[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks is null or Const.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks is null or Const.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheOptions(key, absoluteExpirationTicks, slidingExpirationTicks, data);
    }

    private static DataCacheOptions MapMetadataByAutomatic(string key, RedisValue[] results)
    {
        long? absoluteExpiration = null;
        long? slidingExpiration = null;
        RedisValue data = RedisValue.Null;

        for (int index = 0; index < results.Length; index += 2)
        {
            if (results[index] == Const.ABSOLUTE_EXPIRATION_KEY)
            {
                var absoluteExpirationTicks = (long?)results[index + 1];
                if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != Const.DEADLINE_LASTING)
                {
                    absoluteExpiration = absoluteExpirationTicks.Value;
                }
            }
            else if (results[index] == Const.SLIDING_EXPIRATION_KEY)
            {
                var slidingExpirationTicks = (long?)results[index + 1];
                if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != Const.DEADLINE_LASTING)
                {
                    slidingExpiration = slidingExpirationTicks;
                }
            }
            else if (results[index] == Const.DATA_KEY)
            {
                data = results[index + 1];
            }
        }
        return new DataCacheOptions(key, absoluteExpiration, slidingExpiration, data);
    }

    private static string GetSetExpireArrayOnLua(IEnumerable<KeyValuePair<string, TimeSpan?>> keyValuePairs)
        => "{" + string.Join(',', keyValuePairs.Select(kv => $"'{kv.Key}','{kv.Value?.TotalSeconds ?? -1}'")) + "}";
}
