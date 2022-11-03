// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaException : Exception
{
    public virtual LogLevel? LogLevel { get; set; }

    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Provides error message that I18n is not used
    /// </summary>
    public string? ErrorMessage { get; set; }

    public object[] Parameters { get; private set; }

    public MasaException()
    {
    }

    public MasaException(string message)
        : base(message)
    {
    }

    public MasaException(string errorCode, string? errorMessage, params object[] parameters)
        : this(null, errorCode, errorMessage, parameters)
    {
    }

    public MasaException(Exception? innerException, string errorCode, string? errorMessage, params object[] parameters)
        : base(null, innerException)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        Parameters = parameters;
    }

    public MasaException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public MasaException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
