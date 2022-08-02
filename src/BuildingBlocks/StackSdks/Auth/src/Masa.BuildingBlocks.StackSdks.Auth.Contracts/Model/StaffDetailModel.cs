// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class StaffDetailModel: StaffModel
{
    public List<RoleModel> Roles { get; set; }

    public List<TeamModel> Teams { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }
}

