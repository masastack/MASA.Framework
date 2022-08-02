// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class UserFriendlyException : MasaException
{
    public UserFriendlyException(string message)
        : base(message)
    {
    }
}
