// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Auth.Contracts.Model;

public class UserSimpleModel
{
    public Guid Id { get; set; }

    public string Account { get; set; }

    public string? DisplayName { get; set; }

    public UserSimpleModel(Guid id, string account, string? displayName)
    {
        Id = id;
        Account = account;
        DisplayName = displayName;
    }
}
