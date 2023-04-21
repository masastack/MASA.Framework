// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class ValidateAccountModel : IEnvironmentModel
{
    public string Account { get; set; }

    public string Password { get; set; }

    public bool LdapLogin { get; set; }

    public string Environment { get; set; }
}
