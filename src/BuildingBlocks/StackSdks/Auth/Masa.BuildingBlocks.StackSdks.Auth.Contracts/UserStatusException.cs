// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts;

public class UserStatusException : UserFriendlyException
{
    public UserStatusException(string message) : base(message)
    {
    }

    public UserStatusException(string errorCode, params object[] parameters)
            : base(errorCode, null, parameters)
    {
    }
}
