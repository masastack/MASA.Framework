// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts;

public class MasaUser : IIdentityUser
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public string? Account { get; set; }

    public string[] Roles { get; set; }

    public bool IsSuperAdmin => Account?.ToLower() == "admin";

    public bool IsStaff => StaffId != Guid.Empty;

    public Guid CurrentTeamId { get; set; }

    public Guid StaffId { get; set; }
}
