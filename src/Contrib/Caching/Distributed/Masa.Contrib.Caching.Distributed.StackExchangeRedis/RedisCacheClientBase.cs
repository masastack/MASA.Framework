// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public abstract class RedisCacheClientBase : DistributedCacheClientBase
{
    protected readonly string? InstanceId;
    protected static readonly Guid UniquelyIdentifies = Guid.NewGuid();
    protected readonly ISubscriber Subscriber;

    protected IDatabase Db
    {
        get
        {
            if (_connection.IsConnected || _connection.IsConnecting)
                return _connection.GetDatabase();

            throw new NotSupportedException("Redis service has been disconnected, please wait for reconnection and try again");
        }
    }

    private readonly IConnectionMultiplexer _connection;
    protected readonly JsonSerializerOptions GlobalJsonSerializerOptions;
    private readonly CacheEntryOptions _globalCacheEntryOptions;
    private readonly CacheOptions _globalCacheOptions;

    private readonly string _name;
    private readonly RedisConfigurationOptions? _redisConfigurationOptions;
    private readonly IRedisMultiplexerPool? _redisMultiplexerPool;

    protected RedisCacheClientBase(
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions)
        : this(redisConfigurationOptions.GlobalCacheOptions, redisConfigurationOptions, jsonSerializerOptions)
    {
        var redisConfiguration = redisConfigurationOptions.GetAvailableRedisOptions();
        _connection = ConnectionMultiplexer.Connect(redisConfiguration);
        Subscriber = _connection.GetSubscriber();
        InstanceId = redisConfiguration.InstanceId;
    }

    private RedisCacheClientBase(
        CacheOptions globalCacheOptions,
        CacheEntryOptions expiredEntryOptions,
        JsonSerializerOptions? jsonSerializerOptions)
    {
        _globalCacheOptions = globalCacheOptions;
        _globalCacheEntryOptions = new CacheEntryOptions
        {
            AbsoluteExpiration = expiredEntryOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = expiredEntryOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = expiredEntryOptions.SlidingExpiration
        };
        GlobalJsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions().EnableDynamicTypes();
    }

    protected RedisCacheClientBase(
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        JsonSerializerOptions? jsonSerializerOptions,
        IRedisMultiplexerPool redisMultiplexerProvider)
        : this(redisConfigurationOptions.GlobalCacheOptions, redisConfigurationOptions, jsonSerializerOptions)
    {
        _name = name;
        _redisConfigurationOptions = redisConfigurationOptions.GetAvailableRedisOptions();
        _redisMultiplexerPool = redisMultiplexerProvider;
        _connection = _redisMultiplexerPool.GetConnectionMultiplexer(name, _redisConfigurationOptions);
        Subscriber = _connection.GetSubscriber();
        InstanceId = _redisConfigurationOptions.InstanceId;
    }

    protected T? ConvertToValue<T>(RedisValue value, out bool isExist)
    {
        if (value is { HasValue: true, IsNullOrEmpty: false })
        {
            isExist = true;
            return value.ConvertToValue<T>(GlobalJsonSerializerOptions);
        }

        isExist = false;
        return default;
    }

    protected CacheEntryOptions GetCacheEntryOptions(CacheEntryOptions? options = null)
        => options ?? _globalCacheEntryOptions;

    protected CacheOptions GetCacheOptions(Action<CacheOptions>? action)
    {
        if (action != null)
        {
            CacheOptions cacheOptions = new CacheOptions();
            action.Invoke(cacheOptions);
            return cacheOptions;
        }
        return _globalCacheOptions;
    }

    protected static PublishOptions GetAndCheckPublishOptions(string channel, Action<PublishOptions> setup)
    {
        ArgumentNullException.ThrowIfNull(channel);

        ArgumentNullException.ThrowIfNull(setup);

        var options = new PublishOptions(UniquelyIdentifies);
        setup.Invoke(options);

        MasaArgumentException.ThrowIfNullOrWhiteSpace(options.Key);

        return options;
    }

    internal void RefreshCore(List<DataCacheModel> models, CancellationToken token = default)
    {
        var awaitRefreshOptions = GetKeyAndExpireList(models, token);
        if (awaitRefreshOptions.Count > 0)
        {
            Db.ScriptEvaluate(RedisConstant.SET_EXPIRE_SCRIPT,
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
            await Db.ScriptEvaluateAsync(RedisConstant.SET_EXPIRE_SCRIPT,
                awaitRefreshOptions.Select(item => item.Key).GetRedisKeys(),
                awaitRefreshOptions.Select(item => (RedisValue)(item.Value?.TotalSeconds ?? -1)).ToArray()
            ).ConfigureAwait(false);
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

    private List<DataCacheModel> GetListCoreByKeyPattern(
        string keyPattern,
        Func<string, object, Dictionary<string, RedisResult>> func)
    {
        var arrayRedisResult = func.Invoke(
            RedisConstant.GET_KEY_AND_VALUE_SCRIPT,
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
        string script = getData ? RedisConstant.GET_LIST_SCRIPT : RedisConstant.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = Db
            .ScriptEvaluate(script, keys.Select(key => (RedisKey)key).ToArray())
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult, getData);
    }

    internal async Task<List<DataCacheModel>> GetListAsync(IEnumerable<string> keys, bool getData)
    {
        string script = getData ? RedisConstant.GET_LIST_SCRIPT : RedisConstant.GET_EXPIRATION_VALUE_SCRIPT;
        var arrayRedisResult = (await Db
                .ScriptEvaluateAsync(script, keys.Select(key => (RedisKey)key).ToArray()).ConfigureAwait(false))
            .ToDictionary();
        return GetListByArrayRedisResult(arrayRedisResult, getData);
    }

    private List<DataCacheModel> GetListByArrayRedisResult(Dictionary<string, RedisResult> arrayRedisResult, bool getData)
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

    private DataCacheModel MapMetadata(
        string key,
        RedisValue[] results)
    {
        RedisValue data = results.Length > 2 ? results[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)results[0];
        if (absoluteExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)results[1];
        if (slidingExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheModel(key, absoluteExpirationTicks, slidingExpirationTicks, data, GlobalJsonSerializerOptions);
    }

    private DataCacheModel MapMetadataByAutomatic(string key, RedisValue[] results)
    {
        long? absoluteExpiration = null;
        long? slidingExpiration = null;
        RedisValue data = RedisValue.Null;

        for (int index = 0; index < results.Length; index += 2)
        {
            if (results[index] == RedisConstant.ABSOLUTE_EXPIRATION_KEY)
            {
                var absoluteExpirationTicks = (long?)results[index + 1];
                if (absoluteExpirationTicks.HasValue && absoluteExpirationTicks.Value != RedisConstant.DEADLINE_LASTING)
                {
                    absoluteExpiration = absoluteExpirationTicks.Value;
                }
            }
            else if (results[index] == RedisConstant.SLIDING_EXPIRATION_KEY)
            {
                var slidingExpirationTicks = (long?)results[index + 1];
                if (slidingExpirationTicks.HasValue && slidingExpirationTicks.Value != RedisConstant.DEADLINE_LASTING)
                {
                    slidingExpiration = slidingExpirationTicks;
                }
            }
            else if (results[index] == RedisConstant.DATA_KEY)
            {
                data = results[index + 1];
            }
        }
        return new DataCacheModel(key, absoluteExpiration, slidingExpiration, data, GlobalJsonSerializerOptions);
    }

    protected override void Dispose(bool disposing)
    {
        _redisMultiplexerPool?.TryRemove(_name, _redisConfigurationOptions!);
        _connection?.Dispose();
        base.Dispose(disposing);
    }

    internal CacheExpiredModel GetExpiredInfo(CacheEntryOptions? currentOptions)
    {
        var creationTime = DateTimeOffset.UtcNow;
        var cacheEntryOptions = currentOptions ?? _globalCacheEntryOptions;
        var absoluteExpiration = cacheEntryOptions.GetAbsoluteExpiration(creationTime);
        return new(absoluteExpiration?.Ticks ?? RedisConstant.DEADLINE_LASTING,
            cacheEntryOptions.SlidingExpiration?.Ticks ?? RedisConstant.DEADLINE_LASTING,
            DateTimeOffsetExtensions.GetExpirationInSeconds(creationTime, absoluteExpiration, cacheEntryOptions.SlidingExpiration) ??
            RedisConstant.DEADLINE_LASTING);
    }
}
