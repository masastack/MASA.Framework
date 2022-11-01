// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

[Serializable]
public class UserFriendlyException2 : MasaException2
{
    public UserFriendlyException2(string message)
        : base(message)
    {
    }

    public UserFriendlyException2(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UserFriendlyException2(string errorCode, params object[] parameters)
        : base(errorCode, parameters)
    {
    }

    public UserFriendlyException2(Exception? innerException, string errorCode, params object[] parameters)
        : base(innerException, errorCode, parameters)
    {
    }
}
