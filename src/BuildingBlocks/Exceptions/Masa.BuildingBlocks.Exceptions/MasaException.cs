// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class MasaException2 : Exception
{
    public string? ErrorCode { get; private set; }

    public object[] Parameters { get; private set; }

    public MasaException2()
    {
    }

    public MasaException2(string message)
        : base(message)
    {
    }

    public MasaException2(string errorCode, params object[] parameters)
        : this(null, errorCode, parameters)
    {
    }

    public MasaException2(Exception? innerException, string errorCode, params object[] parameters)
        : base(null, innerException)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }

    public MasaException2(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public MasaException2(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
