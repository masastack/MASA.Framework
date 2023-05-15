// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class ObjectExtensions
{
    /// <summary>
    /// Compressed according to the type or compressed and returned after serialization
    /// </summary>
    /// <param name="value"></param>
    /// <param name="jsonSerializerOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static RedisValue CompressToRedisValue<T>(this T value, JsonSerializerOptions jsonSerializerOptions)
    {
        var type = value?.GetType() ?? typeof(T);
        dynamic redisValue;
        switch (RedisHelper.GetCompressMode(type, out Type actualType))
        {
            case CompressMode.None:
                redisValue = value!;
                break;
            case CompressMode.Compress:
                redisValue = Compress(Encoding.UTF8.GetBytes(value?.ToString() ?? string.Empty));
                break;
            default:
                var jsonString = JsonSerializer.Serialize(value, jsonSerializerOptions);
                redisValue = Compress(Encoding.UTF8.GetBytes(jsonString));
                break;
        }
        return ConvertToRedisValue(actualType, redisValue);
    }

    private static byte[] Compress(byte[] data)
    {
        using MemoryStream msGZip = new MemoryStream();
        using GZipStream stream = new GZipStream(msGZip, CompressionMode.Compress, true);
        stream.Write(data, 0, data.Length);
        stream.Close();
        return msGZip.ToArray();
    }

    private static RedisValue ConvertToRedisValue(Type type, dynamic value)
    {
        if (type == typeof(byte) || type == typeof(ushort))
            return (long)value;

        if (type == typeof(decimal))
            return new RedisValue(value.ToString());

        return value;
    }
}
