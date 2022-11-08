// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class SubjectPermissionRelationModel
{
    public Guid PermissionId { get; set; }

    public bool Effect { get; set; }

    public SubjectPermissionRelationModel(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }

    public override bool Equals(object? obj)
    {
        return obj is SubjectPermissionRelationModel spr && spr.PermissionId == PermissionId;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}
