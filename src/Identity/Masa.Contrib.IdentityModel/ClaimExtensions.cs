// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.IdentityModel;

public static class ClaimExtensions
{
    public static string? FindClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}
