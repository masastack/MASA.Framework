// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel.Const;

public class ClaimType
{
    public const string DEFAULT_USER_ID = ClaimTypes.NameIdentifier;

    public const string DEFAULT_USER_NAME = ClaimTypes.Name;

    public const string DEFAULT_USER_ROLE = ClaimTypes.Role;

    public const string DEFAULT_TENANT_ID = $"{DEFAULT_PREFIX}/tenantid";

    public const string DEFAULT_ENVIRONMENT = $"{DEFAULT_PREFIX}/environment";

    private const string DEFAULT_PREFIX = "https://masastack.com/security/identity/claims";
}
