// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel;

public sealed class DefaultUserContext : UserContext
{
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;

    public DefaultUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor)
        : base(typeConvertProvider)
    {
        _currentPrincipalAccessor = currentPrincipalAccessor;
        _optionsMonitor = optionsMonitor;
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
            UserName = claimsPrincipal.FindClaimValue(_optionsMonitor.CurrentValue.UserName)
        };
    }
}
