// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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

    public UserFriendlyException(string errorCode, params object[] parameters)
        : base(errorCode, parameters)
    {
    }

    public UserFriendlyException(Exception? innerException, string errorCode, params object[] parameters)
        : base(innerException, errorCode, parameters)
    {
    }
}
