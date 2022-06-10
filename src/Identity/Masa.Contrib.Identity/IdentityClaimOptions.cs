// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public class IdentityClaimOptions
{
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string TenantId { get; set; }

    public string Environment { get; set; }

    public IdentityClaimOptions()
    {
        UserId = "sub";
        UserName = "given_name";
        TenantId = "tenant_id";
        Environment = "environment";
    }
}
