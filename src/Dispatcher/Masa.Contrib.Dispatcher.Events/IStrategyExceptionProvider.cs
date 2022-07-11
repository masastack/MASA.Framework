// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events;

public interface IStrategyExceptionProvider
{
    bool SupportRetry(Exception? exception);

    void LogWrite(LogLevel logLevel, Exception? exception, string? message, params object?[] args);
}
