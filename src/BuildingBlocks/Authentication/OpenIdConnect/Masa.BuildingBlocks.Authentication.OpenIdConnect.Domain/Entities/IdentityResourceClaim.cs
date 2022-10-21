// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class IdentityResourceClaim : FullEntity<Guid, Guid>
{
    public Guid UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public Guid IdentityResourceId { get; private set; }

    public IdentityResource IdentityResource { get; private set; } = null!;

    public IdentityResourceClaim(Guid userClaimId)
    {
        UserClaimId = userClaimId;
    }
}

