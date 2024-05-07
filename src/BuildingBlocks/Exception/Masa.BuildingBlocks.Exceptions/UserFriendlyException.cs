// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class UserFriendlyException : MasaException
{
    public UserFriendlyException(string message)
        : base(message)
    {
    }

    public UserFriendlyException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UserFriendlyException(
        string errorCode,
        params object[] parameters)
        : base(errorCode, null, parameters)
    {
    }

    public UserFriendlyException(
        string errorCode,
        LogLevel? logLevel,
        params object[] parameters)
        : base(errorCode, logLevel, parameters)
    {
    }

    public UserFriendlyException(
        string errorCode,
        string message,
        LogLevel? logLevel,
        params object[] parameters)
        : base(message, null, errorCode, logLevel, parameters)
    {
    }

    public UserFriendlyException(
        Exception? innerException,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : base(innerException, errorCode, logLevel, parameters)
    {
    }

    protected UserFriendlyException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
