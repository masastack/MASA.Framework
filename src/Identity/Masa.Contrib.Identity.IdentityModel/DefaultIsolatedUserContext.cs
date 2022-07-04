// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Masa.Contrib.Identity.IdentityModel;

internal class DefaultIsolatedUserContext : DefaultUserContext, IIsolatedUserContext
{
    public string? TenantId => GetUser<IsolatedIdentityUser>()?.TenantId;

    public string? Environment => GetUser<IsolatedIdentityUser>()?.Environment;

    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;

    public DefaultIsolatedUserContext(
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

    protected override IsolatedIdentityUser? GetUser()
    {
        var identityUser = GetUserBasicInfo();
        if (identityUser == null)
        {
            return null;
        }

        return new IsolatedIdentityUser
        {
            Id = identityUser.Id,
            UserName = identityUser.UserName,
            Roles = identityUser.Roles,
            TenantId = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.TenantId),
            Environment = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.Environment),
        };
    }
}
