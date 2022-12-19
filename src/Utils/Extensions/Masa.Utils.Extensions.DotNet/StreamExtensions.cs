// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System.IO;

public static class StreamExtensions
{
    public static byte[] ConvertToBytes(this Stream stream)
    {
        if (!stream.CanRead) return new byte[] { };

        if (!stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    public static async Task<byte[]> ConvertToBytesAsync(this Stream stream)
    {
        if (!stream.CanRead) return new byte[] { };

        if (!stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        var bytes = new byte[stream.Length];
        _ = await stream.ReadAsync(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }
}
