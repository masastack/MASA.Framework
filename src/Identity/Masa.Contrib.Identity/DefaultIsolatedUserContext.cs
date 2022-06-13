// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity;

public sealed class DefaultIsolatedUserContext : UserContext, IIsolatedUserContext
{
    public string? TenantId => GetUser<IsolatedIdentityUser>()?.TenantId;

    public string? Environment => GetUser<IsolatedIdentityUser>()?.Environment;

    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;

    public DefaultIsolatedUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor)
        : base(typeConvertProvider)
    {
        _currentPrincipalAccessor = currentPrincipalAccessor;
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
        var claimsPrincipal = _currentPrincipalAccessor.GetCurrentPrincipal();
        if (claimsPrincipal == null)
            return null;

        var userId = claimsPrincipal.FindClaimValue(_optionsMonitor.CurrentValue.UserId);
        if (userId == null)
            return null;

        return new IsolatedIdentityUser
        {
            Id = userId,
            UserName = claimsPrincipal.FindClaimValue(_optionsMonitor.CurrentValue.UserName),
            TenantId = claimsPrincipal.FindClaimValue(_optionsMonitor.CurrentValue.TenantId),
            Environment = claimsPrincipal.FindClaimValue(_optionsMonitor.CurrentValue.Environment),
        };
    }
}
