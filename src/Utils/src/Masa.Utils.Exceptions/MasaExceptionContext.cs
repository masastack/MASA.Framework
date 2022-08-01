﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class MasaExceptionContext
{
    public Exception Exception { get; set; }

    public HttpContext HttpContext { get; }

    public bool ExceptionHandled { get; set; }

    /// <summary>
    /// Http status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error code to provide support for subsequent I18n
    /// </summary>
    public string? ErrorCode { get; set; }

    public string? Message { get; set; }

    public string ContentType { get; set; }

    internal MasaExceptionContext(Exception exception, HttpContext httpContext)
    {
        Exception = exception;
        HttpContext = httpContext;
        StatusCode = (int)MasaHttpStatusCode.UserFriendlyException;
        ExceptionHandled = false;
        ContentType = Constant.DEFAULT_HTTP_CONTENT_TYPE;
    }

    public void ToResult(
        string message,
        int statusCode = (int)MasaHttpStatusCode.UserFriendlyException,
        string contentType = Constant.DEFAULT_HTTP_CONTENT_TYPE)
    {
        Message = message;
        StatusCode = statusCode;
        ExceptionHandled = true;
        ContentType = contentType;
    }
}
