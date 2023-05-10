// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class TeamDetailModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<StaffModel> Admins { get; set; } = new();

    public List<StaffModel> Members { get; set; } = new();

    public List<RoleModel> AdminRoles { get; set; } = new();

    public List<RoleModel> MemberRoles { get; set; } = new();

    public TeamDetailModel()
    {
        Name = "";
        Avatar = "";
        Description = "";
        Admins = new();
        Members = new();
    }

    public TeamDetailModel(
        Guid id,
        string name,
        string avatar,
        string description,
        TeamTypes teamType,
        List<StaffModel> admins,
        List<StaffModel> members)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Description = description;
        TeamType = teamType;
        Admins = admins;
        Members = members;
    }
}

