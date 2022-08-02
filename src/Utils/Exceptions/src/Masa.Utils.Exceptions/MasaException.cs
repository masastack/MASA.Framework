// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class MasaException : Exception
{
    public string? ErrorCode { get; set; }

    public MasaException()
    {
    }

    public MasaException(string message)
        : base(message)
    {
    }

    public MasaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public MasaException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
