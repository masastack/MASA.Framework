// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.IO;

internal static class StreamExtensions
{
    private static readonly Encoding _defaultEncoding = Encoding.UTF8;

    public static async Task<(long, string?)> ReadAsStringAsync(this Stream stream, Encoding? encoding = null, int bufferSize = 4096)
    {
        if (stream == null || !stream.CanRead || !stream.CanSeek)
            return (-1, null);

        var start = stream.Position;

        try
        {
            List<byte> data = new();
            var buffer = new byte[bufferSize];
            stream.Seek(0, SeekOrigin.Begin);

            do
            {
                var count = await stream.ReadAsync(buffer.AsMemory(0, bufferSize));
                if (count <= 0)
                    break;

                if (bufferSize - count > 0)
                {
                    data.AddRange(buffer[0..count]);
                    break;
                }

                data.AddRange(buffer);
            } while (true);

            if (data.Count > 0)
            {
                if (data.Count - OpenTelemetryInstrumentationOptions.MaxBodySize > 0)
                    return (data.Count, Convert.ToBase64String(data.ToArray()));

                return (data.Count, (encoding ?? _defaultEncoding).GetString(data.ToArray()));
            }
        }
        catch (Exception ex)
        {
            OpenTelemetryInstrumentationOptions.Logger?.LogError(ex, "ReadAsStringAsync Error");
        }
        finally
        {
            stream.Seek(start, SeekOrigin.Begin);
        }

        return (-1, null);
    }
}
