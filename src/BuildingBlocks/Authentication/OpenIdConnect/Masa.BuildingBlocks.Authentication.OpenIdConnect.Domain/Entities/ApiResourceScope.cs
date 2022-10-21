// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiResourceScope : FullEntity<Guid, Guid>
{
    public Guid ApiScopeId { get; private set; }

    public ApiScope ApiScope { get; private set; } = null!;

    public Guid ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;

    public ApiResourceScope(Guid apiScopeId)
    {
        ApiScopeId = apiScopeId;
    }
}

