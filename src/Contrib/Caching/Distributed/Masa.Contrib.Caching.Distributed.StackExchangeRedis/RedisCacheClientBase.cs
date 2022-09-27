// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public abstract class RedisCacheClientBase : DistributedCacheClientBase
{
    protected static readonly Guid UniquelyIdentifies = Guid.NewGuid();
    protected ISubscriber Subscriber;
    protected IDatabase Db;
    protected readonly JsonSerializerOptions JsonSerializerOptions;
    protected CacheEntryOptions CacheEntryOptions;

    protected RedisCacheClientBase(RedisConfigurationOptions redisConfigurationOptions,
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

    private static RedisConfigurationOptions GetRedisConfigurationOptions(RedisConfigurationOptions redisConfigurationOptions)
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

    protected T? ConvertToValue<T>(RedisValue value, out bool isExist)
    {
        if (value.HasValue && !value.IsNullOrEmpty)
        {
            isExist = true;
            return value.ConvertToValue<T>(JsonSerializerOptions);
        }

        isExist = false;
        return default;
    }

    protected CacheEntryOptions GetCacheEntryOptions(CacheEntryOptions? options = null)
        => options ?? CacheEntryOptions;

    protected static PublishOptions GetAndCheckPublishOptions(string channel, Action<PublishOptions> setup)
    {
        ArgumentNullException.ThrowIfNull(channel);

        ArgumentNullException.ThrowIfNull(setup);

        var options = new PublishOptions(UniquelyIdentifies);
        setup.Invoke(options);

        options.Key.CheckIsNullOrWhiteSpace();

        return options;
    }

    internal void RefreshCore(List<DataCacheModel> models, CancellationToken token = default)
    {
        var awaitRefreshOptions = GetKeyAndExpireList(models, token);
        if (awaitRefreshOptions.Count > 0)
        {
            Db.ScriptEvaluate(Const.SET_EXPIRE_SCRIPT,
                awaitRefreshOptions.Select(item => item.Key).GetRedisKeys(),
                awaitRefreshOptions.Select(item => (RedisValue)(item.Value?.TotalSeconds ?? -1)).ToArray()
            );
        }
    }

    internal async Task RefreshCoreAsync(
        List<DataCacheModel> models,
        CancellationToken token = default)
    {
        var awaitRefreshOptions = GetKeyAndExpireList(models, token);
        if (awaitRefreshOptions.Count > 0)
        {
            await Db.ScriptEvaluateAsync(Const.SET_EXPIRE_SCRIPT,
                awaitRefreshOptions.Select(item => item.Key).GetRedisKeys(),
                awaitRefreshOptions.Select(item => (RedisValue)(item.Value?.TotalSeconds ?? -1)).ToArray()
            );
        }
    }

    internal List<DataCacheModel> GetListByKeyPattern(string keyPattern)
        => GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db.ScriptEvaluate(LuaScript.Prepare(script), parameters)
            .ToDictionary());

    internal Task<List<DataCacheModel>> GetListByKeyPatternAsync(string keyPattern)
        => Task.FromResult(GetListCoreByKeyPattern(keyPattern, (script, parameters) => Db
            .ScriptEvaluateAsync(LuaScript.Prepare(script), parameters)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
            .ToDictionary()));

    private static List<DataCacheModel> GetListCoreByKeyPattern(
        string keyPattern,
        Func<string, object, Dictionary<string, RedisResult>> func)
    {
        var arrayRedisResult = func.Invoke(
            Const.GET_KEY_AND_VALUE_SCRIPT,
            new
            {
                keypattern = keyPattern
            });

        List<DataCacheModel> list = new List<DataCacheModel>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(MapMetadataByAutomatic(redisResult.Key, byteArray));
        }
        return list;
    }

    internal List<DataCacheModel> GetList(IEnumerable<string> keys, bool getData)
    {
        string script = getData ? Const.GET_LIST_SCRIPT : Const.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = Db
            .ScriptEvaluate(script, keys.Select(key => (RedisKey)key).ToArray())
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult, getData);
    }

    internal async Task<List<DataCacheModel>> GetListAsync(IEnumerable<string> keys, bool getData)
    {
        string script = getData ? Const.GET_LIST_SCRIPT : Const.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = (await Db
                .ScriptEvaluateAsync(script, keys.Select(key => (RedisKey)key).ToArray()))
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult, getData);
    }

    private static List<DataCacheModel> GetListByArrayRedisResult(Dictionary<string, RedisResult> arrayRedisResult, bool getData)
    {
        List<DataCacheModel> list = new List<DataCacheModel>();
        foreach (var redisResult in arrayRedisResult)
        {
            var byteArray = (RedisValue[])redisResult.Value;
            list.Add(getData ? MapMetadataByAutomatic(redisResult.Key, byteArray) : MapMetadata(redisResult.Key, byteArray));
        }
        return list;
    }

    private static List<KeyValuePair<string, TimeSpan?>> GetKeyAndExpireList(
        List<DataCacheModel> models,
        CancellationToken token)
    {
        List<KeyValuePair<string, TimeSpan?>> list = new();

        DateTimeOffset? creationTime = DateTimeOffset.UtcNow;
        foreach (var model in models)
        {
            var res = model.GetExpiration(creationTime, token);
            if (res.State)
            {
                list.Add(new KeyValuePair<string, TimeSpan?>(model.Key, res.Expire));
            }
        }
        return list;
    }

    internal static DataCacheModel MapMetadata(
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
        return new DataCacheModel(key, absoluteExpirationTicks, slidingExpirationTicks, data);
    }

    private static DataCacheModel MapMetadataByAutomatic(string key, RedisValue[] results)
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
        return new DataCacheModel(key, absoluteExpiration, slidingExpiration, data);
    }
}
