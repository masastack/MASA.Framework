// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class LoginByPhoneNumberModel : IEnvironmentModel
{
    public string PhoneNumber { get; set; } = "";

    public string Code { get; set; } = "";

    public bool RegisterLogin { get; set; }

    public string Environment { get; set; }
}
