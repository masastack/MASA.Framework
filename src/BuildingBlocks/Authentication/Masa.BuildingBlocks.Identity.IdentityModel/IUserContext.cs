// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Identity.IdentityModel;

public interface IUserContext
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    string? UserName { get; }

    TUserId? GetUserId<TUserId>();

    TIdentityUser? GetUser<TIdentityUser>() where TIdentityUser : IIdentityUser;

    IEnumerable<IdentityRole<TRoleId>> GetUserRoles<TRoleId>();
}
