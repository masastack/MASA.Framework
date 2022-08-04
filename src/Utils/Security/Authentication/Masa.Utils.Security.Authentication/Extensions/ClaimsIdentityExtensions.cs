// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication.Extensions;

public static class ClaimsIdentityExtensions
{
    public static string? FindClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        => claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

    public static Guid? FindUserId(this ClaimsPrincipal principal)
    {
        var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == MasaClaimTypes.USER_ID);
        if (userIdOrNull == null || string.IsNullOrWhiteSpace(userIdOrNull.Value))
        {
            return null;
        }

        if (Guid.TryParse(userIdOrNull.Value, out Guid guid))
        {
            return guid;
        }

        return null;
    }

    public static Guid? FindTenantId(this ClaimsPrincipal principal)
    {
        var tenantIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == MasaClaimTypes.TENANT);
        if (tenantIdOrNull == null || string.IsNullOrWhiteSpace(tenantIdOrNull.Value))
        {
            return null;
        }

        if (Guid.TryParse(tenantIdOrNull.Value, out var guid))
        {
            return guid;
        }

        return null;
    }

    public static string FindEnvironment(this ClaimsPrincipal principal)
    {
        var evironmentOrNull = principal.Claims?.FirstOrDefault(c => c.Type == MasaClaimTypes.ENVIRONMENT);
        if (evironmentOrNull == null || string.IsNullOrWhiteSpace(evironmentOrNull.Value))
        {
            return string.Empty;
        }

        return evironmentOrNull.Value;
    }
}
