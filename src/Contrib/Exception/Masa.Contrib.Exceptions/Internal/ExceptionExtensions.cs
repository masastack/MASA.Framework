// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

internal static class ExceptionExtensions
{
    public static void WriteLog(
        this ILogger logger,
        Exception exception,
        LogLevel defaultLogLevel,
        MasaExceptionLogRelationOptions logRelationOptions,
        string? message = null)
    {
        var logLevel = logRelationOptions.GetLogLevel(exception, defaultLogLevel);
        logger.Log(logLevel, exception, message ?? exception.Message);
    }
}
