// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities;

public class IdentityResourceClaim : FullEntity<int, Guid>
{
    public int UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public int IdentityResourceId { get; private set; }

    public IdentityResource IdentityResource { get; private set; } = null!;

    public IdentityResourceClaim(int userClaimId)
    {
        UserClaimId = userClaimId;
    }
}

