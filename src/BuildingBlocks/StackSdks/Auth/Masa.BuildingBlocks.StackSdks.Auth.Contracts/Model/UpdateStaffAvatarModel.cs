// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UpdateStaffAvatarModel
{
    public Guid UserId { get; set; }

    public string Avatar { get; set; }

    public UpdateStaffAvatarModel(Guid userId, string avatar)
    {
        UserId = userId;
        Avatar = avatar;
    }
}
