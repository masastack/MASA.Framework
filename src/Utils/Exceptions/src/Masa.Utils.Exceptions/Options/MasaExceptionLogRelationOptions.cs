// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class MasaExceptionLogRelationOptions
{
    internal Dictionary<Type, LogLevel> Mappings { get; } = new();

    public MasaExceptionLogRelationOptions MapLogLevel<TException>(LogLevel logLevel) where TException : Exception
    {
        Mappings[typeof(TException)] = logLevel;
        return this;
    }

    internal LogLevel GetLogLevel(Exception exception, LogLevel defaultLogLevel)
    {
        if (!Mappings.TryGetValue(exception.GetType(), out var logLevel))
            logLevel = defaultLogLevel;

        return logLevel;
    }
}
