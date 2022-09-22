// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class RegisterByPhoneModel : RegisterModel
{
    public UserRegisterTypes UserRegisterType { get; set; } = UserRegisterTypes.PhoneNumber;

    public string PhoneNumber { get; set; }

    public string SmsCode { get; set; }
}
