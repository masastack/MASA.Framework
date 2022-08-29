// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class UpdateUserAvatarModel
{
    public Guid Id { get; set; }

    public string Avatar { get; set; }

    public UpdateUserAvatarModel(Guid id, string avatar)
    {
        Id = id;
        Avatar = avatar;
    }
}
