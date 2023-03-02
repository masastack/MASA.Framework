// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[ExcludeFromCodeCoverage]
internal static class LoggerExtensions
{
    public static void WriteLog(
        this ILogger logger,
        Exception exception,
        MasaExceptionLogRelationOptions logRelationOptions,
        string? message = null)
    {
        LogLevel defaultLogLevel = LogLevel.Error;
        if (exception is MasaException masaException)
        {
            if (masaException.LogLevel != null)
            {
                logger.Log(masaException.LogLevel!.Value, exception, "{Message}", message);
                return;
            }
            if (masaException is UserFriendlyException)
                defaultLogLevel = LogLevel.Information;
            if (masaException is BackgroundJobException)
                defaultLogLevel = LogLevel.Error;
        }
        var logLevel = logRelationOptions.GetLogLevel(exception, defaultLogLevel);
        logger.Log(logLevel, exception, "{Message}", message);
    }
}
