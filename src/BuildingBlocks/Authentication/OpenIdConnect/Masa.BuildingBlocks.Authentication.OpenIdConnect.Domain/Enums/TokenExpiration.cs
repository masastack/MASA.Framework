// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Enums;

/// <summary>
/// Token expiration types.
/// </summary>
public enum TokenExpiration
{
    /// <summary>
    /// Sliding token expiration
    /// </summary>
    Sliding = 0,

    /// <summary>
    /// Absolute token expiration
    /// </summary>
    Absolute = 1
}
