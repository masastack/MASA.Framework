// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UpdateUserPhoneNumberModel
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; }

    public string VerificationCode { get; set; }

    public UpdateUserPhoneNumberModel(Guid id, string phoneNumber, string verificationCode)
    {
        Id = id;
        PhoneNumber = phoneNumber;
        VerificationCode = verificationCode;
    }
}
