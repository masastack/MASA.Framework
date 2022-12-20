// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Runtime.Serialization;

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts;

[Serializable]
public class UserStatusException : UserFriendlyException
{
    public UserStatusException(string message) : base(message)
    {
    }

    public UserStatusException(string errorCode, params object[] parameters)
            : base(errorCode, null, parameters)
    {
    }

    protected UserStatusException(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
    {
    }
}
