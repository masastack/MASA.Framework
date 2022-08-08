// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity;

internal class DefaultUserContext : UserContext
{
    private readonly IOptionsMonitor<IdentityClaimOptions> _optionsMonitor;
    private readonly ILogger<DefaultUserContext>? _logger;

    protected ClaimsPrincipal? ClaimsPrincipal { get; set; }

    public DefaultUserContext(
        ITypeConvertProvider typeConvertProvider,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IOptionsMonitor<IdentityClaimOptions> optionsMonitor, ILoggerFactory? loggerFactory = null)
        : base(typeConvertProvider)
    {
        _optionsMonitor = optionsMonitor;
        ClaimsPrincipal = currentPrincipalAccessor.GetCurrentPrincipal();
        _logger = loggerFactory?.CreateLogger<DefaultUserContext>();
    }

    protected override IdentityUser? GetUser()
    {
        return GetUserBasicInfo();
    }

    protected override IdentityUser? GetUserBasicInfo()
    {

        var userId = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.UserId);
        if (userId == null)
            return null;

        var roleStr = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.Role);
        var roles = Array.Empty<string>();
        if (!string.IsNullOrWhiteSpace(roleStr))
        {
            try
            {
                roles = JsonSerializer.Deserialize<string[]>(roleStr) ?? roles;
            }
            catch (Exception e)
            {
                _logger?.LogError("role data deserialization failed", e);
            }
        }
        return new IdentityUser
        {
            Id = userId,
            UserName = ClaimsPrincipal?.FindClaimValue(_optionsMonitor.CurrentValue.UserName),
            Roles = roles
        };
    }
}
