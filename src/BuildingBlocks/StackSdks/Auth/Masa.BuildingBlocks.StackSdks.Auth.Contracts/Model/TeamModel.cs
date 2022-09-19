// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class TeamModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string Description { get; set; } = "";

    public RoleModel Role { get; set; } = new();

    public int MemberCount { get; set; }
}
