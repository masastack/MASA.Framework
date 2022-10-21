// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiResourceClaim : FullEntity<Guid, Guid>
{
    public Guid UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public Guid ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;

    public ApiResourceClaim(Guid userClaimId)
    {
        UserClaimId = userClaimId;
    }
}

