// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiScopeClaim : FullEntity<Guid, Guid>
{
    public Guid UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public Guid ApiScopeId { get; private set; }

    public ApiScope ApiScope { get; private set; } = null!;

    public ApiScopeClaim(Guid userClaimId)
    {
        UserClaimId = userClaimId;
    }
}
