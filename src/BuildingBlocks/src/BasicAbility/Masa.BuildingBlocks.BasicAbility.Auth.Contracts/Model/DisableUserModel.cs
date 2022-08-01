// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;

public class DisableUserModel
{
    public string Account { get; set; }

    public DisableUserModel(string account)
    {
        Account = account;
    }
}
