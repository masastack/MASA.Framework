// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

[Serializable]
public class MasaException : Exception
{
    public string? ErrorCode { get; set; }

    public object[] Parameters { get; set; }

    public MasaException()
    {
    }

    public MasaException(string message)
        : base(message)
    {
    }

    public MasaException(string errorCode, params object[] parameters)
        : this(null, errorCode, parameters)
    {
    }

    public MasaException(Exception? innerException, string errorCode, params object[] parameters)
        : base(null, innerException)
    {
        ErrorCode = errorCode;
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
