﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public class DefaultStrategyExceptionProvider : IStrategyExceptionProvider
{
    private readonly ILogger<DefaultStrategyExceptionProvider>? _logger;

    public DefaultStrategyExceptionProvider(ILogger<DefaultStrategyExceptionProvider>? logger = null)
        => _logger = logger;

    /// <summary>
    /// Default UserFriendlyException does not support retry
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public bool SupportRetry(Exception? exception)
    {
        return exception is not UserFriendlyException;
    }

    public void LogWrite(LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
        if (exception is not UserFriendlyException)
            _logger?.Log(logLevel, exception, message, args);
    }
}
