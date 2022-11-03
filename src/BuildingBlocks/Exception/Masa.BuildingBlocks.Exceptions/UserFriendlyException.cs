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

    public UserFriendlyException(string errorCode, string errorMessage, params object[] parameters)
        : base(errorCode, errorMessage, parameters)
    {
    }

    public UserFriendlyException(Exception? innerException, string errorCode, string errorMessage, params object[] parameters)
        : base(innerException, errorCode, errorMessage, parameters)
    {
    }
}
