// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class RedisValueExtensions
{
    public static HashEntry[] ConvertToHashEntries(
        this RedisValue redisValue,
        CacheExpiredModel cacheExpiredModel)
    {
        var hashEntries = new List<HashEntry>()
        {
            new(RedisConstant.ABSOLUTE_EXPIRATION_KEY, cacheExpiredModel.AbsoluteExpirationTicks),
            new(RedisConstant.SLIDING_EXPIRATION_KEY, cacheExpiredModel.SlidingExpirationTicks),
            new HashEntry(RedisConstant.DATA_KEY, redisValue)
        };

        return hashEntries.ToArray();
    }

    public static T? DecompressToValue<T>(this RedisValue redisValue, JsonSerializerOptions jsonSerializerOptions)
    {
        var type = typeof(T);
        var compressMode = RedisHelper.GetCompressMode(type, out Type actualType);

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

    private static byte[] Decompress(byte[] data)
    {
        using MemoryStream ms = new MemoryStream(data);
        using GZipStream stream = new GZipStream(ms, CompressionMode.Decompress);
        using MemoryStream outBuffer = new MemoryStream();
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
