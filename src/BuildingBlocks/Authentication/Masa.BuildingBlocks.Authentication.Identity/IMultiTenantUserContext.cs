// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

/// <summary>
/// After the user logs in, the tenant information of the current user can be obtained
/// </summary>
public interface IMultiTenantUserContext : IUserContext
{
    string? TenantId { get; }

    TTenantId? GetTenantId<TTenantId>();
}
