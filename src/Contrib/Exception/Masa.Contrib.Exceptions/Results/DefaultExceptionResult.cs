// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Mvc;

[ExcludeFromCodeCoverage]
public class DefaultExceptionResult : IActionResult
{
    public string Message { get; set; }

    public int StatusCode { get; set; }

    public string ContentType { get; set; }

    public DefaultExceptionResult(string message, int statusCode, string contentType)
    {
        Message = message;
        StatusCode = statusCode;
        ContentType = contentType;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        await context.HttpContext.Response.WriteTextAsync(StatusCode, Message, ContentType);
    }
}
