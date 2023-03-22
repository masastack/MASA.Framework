// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisHelper
{
    public static T? DecompressToValue<T>(this RedisValue redisValue, JsonSerializerOptions jsonSerializerOptions)
    {
        var type = typeof(T);
        var compressMode = GetCompressMode(type, out Type actualType);

        if (compressMode == CompressMode.None)
            return (T?)Convert.ChangeType(redisValue, actualType);

        var byteValue = (byte[])redisValue;
        if (byteValue.Length == 0)
            return default;

        var value = Decompress(byteValue);

        if (compressMode == CompressMode.Compress)
        {
            var valueString = Encoding.UTF8.GetString(value);
            return (dynamic)valueString;
        }

        return JsonSerializer.Deserialize<T>(value, jsonSerializerOptions);
    }

    public static RedisKey[] GetRedisKeys(this IEnumerable<string> keys)
        => keys.Select(key => (RedisKey)key).ToArray();

    public static byte[] Decompress(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        using (GZipStream stream = new GZipStream(ms, CompressionMode.Decompress))
        using (MemoryStream outBuffer = new MemoryStream())
        {
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            return outBuffer.ToArray();
        }
    }

    private static CompressMode GetCompressMode(this Type type, out Type actualType)
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
            else if (hashEntry.Name == RedisConstant.ABSOLUTE_EXPIRATION_KEY)
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
