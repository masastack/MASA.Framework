// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel;

public class IdentityClaimOptions
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string Role { get; set; }

    public string TenantId { get; set; }

    public string Environment { get; set; }

    public IdentityClaimOptions()
    {
        UserId = ClaimType.DEFAULT_USER_ID;
        UserName = ClaimType.DEFAULT_USER_NAME;
        Role = ClaimType.DEFAULT_USER_ROLE;
        TenantId = ClaimType.DEFAULT_TENANT_ID;
        Environment = ClaimType.DEFAULT_ENVIRONMENT;
    }
}
