// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.IO;

public static class StreamExtensions
{
    private static readonly Encoding _defaultEncoding = Encoding.UTF8;

    public static async Task<string?> ReadAsStringAsync(this Stream stream, Encoding? encoding = null)
    {
        if (stream == null)
            return null;

        if(!stream.CanRead)
            return "cann't read";

        if (!stream.CanSeek)
            return "cann't seek";
       
        var start = (int)stream.Position;
        List<byte> data = new();
        var buffer = new byte[1024];
        do
        {
            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (count <= 0)
                break;
            if (buffer.Length - count == 0)
            {
                data.AddRange(buffer);
            }
            else
            {
                data.AddRange(buffer[0..count]);
                break;
            }            
        } while (true);

        if (data.Count > 0)
        {
            stream.Seek(start, SeekOrigin.Begin);
            return (encoding ?? _defaultEncoding).GetString(data.ToArray());
        }

        return null;
    }
}

