// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal class DataCacheBaseModel
{
    public string Key { get; }

    public long? AbsoluteExpiration { get; }

    public long? SlidingExpiration { get; }

    public DataCacheBaseModel(string key, long? absoluteExpiration, long? slidingExpiration)
    {
        Key = key;
        AbsoluteExpiration = absoluteExpiration;
        SlidingExpiration = slidingExpiration;
    }
}

/// <summary>
/// Data stored to Redis
/// </summary>
internal class DataCacheModel : DataCacheBaseModel
{
    public RedisValue RedisValue { get; }

    protected JsonSerializerOptions JsonSerializerOptions { get; }

    public bool IsExist { get; }

    public DataCacheModel(
        string key,
        long? absoluteExpiration,
        long? slidingExpiration,
        RedisValue redisValue,
        JsonSerializerOptions jsonSerializerOptions) : base(key, absoluteExpiration, slidingExpiration)
    {
        RedisValue = redisValue;
        JsonSerializerOptions = jsonSerializerOptions;
        IsExist = redisValue is { HasValue: true, IsNullOrEmpty: false };
    }
}

internal class DataCacheModel<T> : DataCacheModel
{
    public T? Value { get; private set; }

    public DataCacheModel(
        string key,
        long? absoluteExpiration,
        long? slidingExpiration,
        RedisValue redisValue,
        JsonSerializerOptions jsonSerializerOptions)
        : base(key, absoluteExpiration, slidingExpiration, redisValue, jsonSerializerOptions)
    {
        if (IsExist) Value = RedisValue.DecompressToValue<T>(JsonSerializerOptions);
    }

    public void TrySetValue(Action existAction, Func<T>? notExistFunc)
    {
        if (IsExist) existAction.Invoke();

        else if (notExistFunc != null) Value = notExistFunc.Invoke();
    }

    public async Task TrySetValueAsync(Action existAction, Func<Task<T>>? notExistFunc)
    {
        if (IsExist) existAction.Invoke();

        else if (notExistFunc != null) Value = await notExistFunc.Invoke();
    }
}
