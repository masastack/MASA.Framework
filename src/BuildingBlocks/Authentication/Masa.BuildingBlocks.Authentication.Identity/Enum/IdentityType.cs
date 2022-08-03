// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Identity;

[Flags]
public enum IdentityType
{
    /// <summary>
    /// Only use user information
    /// </summary>
    Basic = 0x01,

    /// <summary>
    /// Multi-tenant
    /// </summary>
    MultiTenant = 0x02,

    /// <summary>
    /// Multi-Environment
    /// </summary>
    MultiEnvironment = 0x04,
}
