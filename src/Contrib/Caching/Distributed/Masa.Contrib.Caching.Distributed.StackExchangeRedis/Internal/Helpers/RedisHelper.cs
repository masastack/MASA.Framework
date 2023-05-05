// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisHelper
{
    internal static CompressMode GetCompressMode(Type type, out Type actualType)
    {
        actualType = Nullable.GetUnderlyingType(type) ?? type;

        switch (Type.GetTypeCode(actualType))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Double:
            case TypeCode.Single:
            case TypeCode.Decimal:
                return CompressMode.None;
            case TypeCode.String:
                return CompressMode.Compress;
            default:
                return CompressMode.SerializeAndCompress;
        }
    }

    public static RedisKey[] GetRedisKeys(this IEnumerable<string> keys)
        => keys.Select(key => (RedisKey)key).ToArray();

    public static DataCacheBaseModel ConvertToCacheBaseModel(string key, RedisValue[] values)
    {
        var absoluteExpirationTicks = (long?)values[0];
        if (absoluteExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)values[1];
        if (slidingExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheBaseModel(key, absoluteExpirationTicks, slidingExpirationTicks);
    }

    public static DataCacheModel<T> ConvertToCacheModel<T>(
        string key,
        RedisValue[] values,
        JsonSerializerOptions jsonSerializerOptions)
    {
        RedisValue data = values.Length > 2 ? values[2] : RedisValue.Null;
        var absoluteExpirationTicks = (long?)values[0];
        if (absoluteExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            absoluteExpirationTicks = null;

        var slidingExpirationTicks = (long?)values[1];
        if (slidingExpirationTicks is null or RedisConstant.DEADLINE_LASTING)
            slidingExpirationTicks = null;
        return new DataCacheModel<T>(key, absoluteExpirationTicks, slidingExpirationTicks, data, jsonSerializerOptions);
    }

    public static DataCacheModel<T> ConvertToCacheModel<T>(
        string key,
        HashEntry[] hashEntries,
        JsonSerializerOptions jsonSerializerOptions)
    {
        var item = FormatHashEntries(hashEntries);
        return new DataCacheModel<T>(
            key,
            item.AbsoluteExpirationTicks,
            item.SlidingExpirationTicks,
            item.RedisValue,
            jsonSerializerOptions);
    }

    private static (long? AbsoluteExpirationTicks, long? SlidingExpirationTicks, RedisValue RedisValue) FormatHashEntries(
        HashEntry[] hashEntries)
    {
        long? absoluteExpiration = null;
        long? slidingExpiration = null;
        RedisValue data = RedisValue.Null;
        foreach (var hashEntry in hashEntries)
        {
            if (hashEntry.Name == RedisConstant.ABSOLUTE_EXPIRATION_KEY)
            {
                if (hashEntry.Value.HasValue && hashEntry.Value != RedisConstant.DEADLINE_LASTING)
                {
                    absoluteExpiration = (long?)hashEntry.Value;
                }
            }
            else if (hashEntry.Name == RedisConstant.SLIDING_EXPIRATION_KEY)
            {
                if (hashEntry.Value.HasValue && hashEntry.Value != RedisConstant.DEADLINE_LASTING)
                {
                    slidingExpiration = (long?)hashEntry.Value;
                }
            }
            else if (hashEntry.Name == RedisConstant.DATA_KEY)
            {
                data = hashEntry.Value;
            }
        }
        return new(absoluteExpiration, slidingExpiration, data);
    }
}
