// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class RegisterByEmailModel : RegisterByPhoneModel, IEnvironmentModel
{
    public RegisterByEmailModel()
    {
        UserRegisterType = UserRegisterTypes.Email;
    }

    public string Email { get; set; }

    public string EmailCode { get; set; }

    public string Environment { get; set; }
}
