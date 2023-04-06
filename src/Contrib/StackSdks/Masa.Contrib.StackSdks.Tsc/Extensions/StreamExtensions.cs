// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System.IO;

internal static class StreamExtensions
{
    private static readonly Encoding _defaultEncoding = Encoding.UTF8;

    public static async Task<(long, string?)> ReadAsStringAsync(this Stream stream, Encoding? encoding = null, int bufferSize = 4096)
    {
        if (stream == null)
            return (-1, null);

        if (!stream.CanRead)
            return (-1, "cann't read");

        if (!stream.CanSeek)
            return (-1, "cann't seek");

        var start = stream.Position;

        try
        {
            List<byte> data = new();
            var buffer = new byte[bufferSize];
            stream.Seek(0, SeekOrigin.Begin);

            do
            {
                var count = await stream.ReadAsync(buffer, 0, bufferSize);
                if (count <= 0)
                    break;
                if (bufferSize - count == 0)
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
                if (data.Count - OpenTelemetryInstrumentationOptions.MaxBodySize > 0)
                {
                    return (data.Count, Convert.ToBase64String(data.ToArray()));
                }
                else
                {
                    return (data.Count, (encoding ?? _defaultEncoding).GetString(data.ToArray()));
                }
            }
        }
        catch (Exception ex)
        {
            OpenTelemetryInstrumentationOptions.Logger?.LogError("ReadAsStringAsync", ex);
        }
        finally
        {
            if (stream != null && stream.CanSeek)
                stream.Seek(start, SeekOrigin.Begin);
        }

        return (-1, null);
    }
}
