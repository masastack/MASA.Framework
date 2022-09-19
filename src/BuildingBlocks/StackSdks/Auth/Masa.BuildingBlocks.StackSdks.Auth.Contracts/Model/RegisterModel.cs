// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class RegisterModel
{
    public UserRegisterTypes UserRegisterType { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string PhoneNumber { get; set; }

    public string Code { get; set; }

    public string Account { get; set; }

    public string Avatar { get; set; }

    public string DisplayName { get; set; }
}
