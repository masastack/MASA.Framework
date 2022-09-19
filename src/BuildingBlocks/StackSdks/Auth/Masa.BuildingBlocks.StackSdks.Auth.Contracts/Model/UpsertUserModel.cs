// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UpsertUserModel : AddUserModel
{
    public Guid Id { get; set; }

    public List<RoleModel> Roles { get; set; } = new();
}

