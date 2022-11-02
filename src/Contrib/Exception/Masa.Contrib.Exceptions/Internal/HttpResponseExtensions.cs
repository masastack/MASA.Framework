// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Exceptions.Internal;

internal static class HttpResponseExtensions
{
    /// <summary>
    /// Write response with text/plain
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="statusCode"></param>
    /// <param name="text"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task WriteTextAsync(
        this HttpResponse httpResponse,
        int statusCode,
        string text,
        string contentType = "text/plain; charset=utf-8")
    {
        httpResponse.StatusCode = statusCode;
        httpResponse.ContentType = contentType;
        await httpResponse.WriteAsync(text, Encoding.UTF8);
    }
}
