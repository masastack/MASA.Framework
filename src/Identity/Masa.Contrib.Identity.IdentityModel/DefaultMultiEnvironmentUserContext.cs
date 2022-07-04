// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Masa.Contrib.Identity.IdentityModel;

internal class DefaultMultiEnvironmentUserContext : DefaultUserContext, IMultiEnvironmentUserContext
{
    public string? Environment => GetUser<MultiEnvironmentIdentityUser>()?.Environment;

    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;

    public DefaultMultiEnvironmentUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor,
        ILoggerFactory? loggerFactory = null)
        : base(typeConvertProvider, currentPrincipalAccessor, optionsMonitor, loggerFactory)
    {
        _optionsMonitor = optionsMonitor;
    }

    protected override MultiEnvironmentIdentityUser? GetUser()
    {
        var identityUser = GetUserBasicInfo();
        if (identityUser == null)
        {
            return null;
        }

        return new MultiEnvironmentIdentityUser
        {
            Id = identityUser.Id,
            UserName = identityUser.UserName,
            Roles = identityUser.Roles,
            Environment = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.Environment),
        };
    }
}
