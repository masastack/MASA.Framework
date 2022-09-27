// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

public class DefaultMultiTenantUserContext : BaseUserContext, IMultiTenantUserContext
{
    public string? TenantId => GetUser<MultiTenantIdentityUser>()?.TenantId;

    public DefaultMultiTenantUserContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public TTenantId? GetTenantId<TTenantId>()
    {
        var tenantId = TenantId;
        if (tenantId == null)
            return default;

        return TypeConvertProvider.ConvertTo<TTenantId>(tenantId);
    }
}
