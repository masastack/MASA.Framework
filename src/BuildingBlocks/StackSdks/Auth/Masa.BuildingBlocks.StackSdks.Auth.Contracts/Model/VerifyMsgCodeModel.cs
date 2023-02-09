// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class VerifyMsgCodeModel
{
    public Guid UserId { get; set; }

    public string Code { get; set; }

    public string? PhoneNumber { get; set; }

    public SendMsgCodeTypes SendMsgCodeType { get; set; } = SendMsgCodeTypes.VerifiyPhoneNumber;
}
