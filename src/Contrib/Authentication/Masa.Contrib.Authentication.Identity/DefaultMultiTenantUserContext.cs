// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

internal sealed class DefaultMultiTenantUserContext : DefaultUserContext, IMultiTenantUserContext
{
    public string? TenantId => GetUser<MultiTenantIdentityUser>()?.TenantId;

    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;

    public DefaultMultiTenantUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor,
        ILoggerFactory? loggerFactory = null)
        : base(typeConvertProvider, currentPrincipalAccessor, optionsMonitor, loggerFactory)
    {
        _optionsMonitor = optionsMonitor;
    }

    public TTenantId? GetTenantId<TTenantId>()
    {
        var tenantId = TenantId;
        if (tenantId == null)
            return default;

        return TypeConvertProvider.ConvertTo<TTenantId>(tenantId);
    }

    protected override MultiTenantIdentityUser? GetUser()
    {
        var identityUser = GetUserBasicInfo();
        if (identityUser == null)
        {
            return null;
        }

        return new MultiTenantIdentityUser
        {
            Id = identityUser.Id,
            UserName = identityUser.UserName,
            Roles = identityUser.Roles,
            TenantId = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.TenantId),
        };
    }
}
