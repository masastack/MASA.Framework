// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Identity.IdentityModel;

public interface IMultiTenantUserContext : IUserContext
{
    string? TenantId { get; }

    TTenantId? GetTenantId<TTenantId>();
}
