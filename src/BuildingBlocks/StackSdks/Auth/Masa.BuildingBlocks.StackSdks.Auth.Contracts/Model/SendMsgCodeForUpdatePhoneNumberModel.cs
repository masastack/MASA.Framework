// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class SendMsgCodeForUpdatePhoneNumberModel
{
    public Guid UserId { get; set; }

    public string PhoneNumber { get; set; }

    public SendMsgCodeForUpdatePhoneNumberModel(Guid userId, string phoneNumber)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
    }
}
