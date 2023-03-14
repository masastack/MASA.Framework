// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobException : MasaException
{
    public BackgroundJobException(string message)
        : base(message)
    {
    }

    public BackgroundJobException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public BackgroundJobException(
        string errorCode,
        params object[] parameters)
        : base(errorCode, null, parameters)
    {
    }

    public BackgroundJobException(
        string errorCode,
        LogLevel? logLevel,
        params object[] parameters)
        : base(errorCode, logLevel, parameters)
    {
    }

    public BackgroundJobException(
        Exception? innerException,
        string errorCode,
        LogLevel? logLevel = null,
        params object[] parameters)
        : base(innerException, errorCode, logLevel, parameters)
    {
    }

    protected BackgroundJobException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
