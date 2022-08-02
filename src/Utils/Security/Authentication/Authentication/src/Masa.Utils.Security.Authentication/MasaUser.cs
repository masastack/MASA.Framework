// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication;

public class MasaUser
{
    readonly ICurrentPrincipalAccessor _principalAccessor;

    public MasaUser(ICurrentPrincipalAccessor principalAccessor)
    {
        _principalAccessor = principalAccessor;
    }

    public virtual Claim? FindClaim(string claimType)
    {
        return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? new Claim[0];
    }

    public virtual Claim[] GetAllClaims()
    {
        return _principalAccessor.Principal?.Claims.ToArray() ?? new Claim[0];
    }

    public Guid? UserId => _principalAccessor.Principal?.FindUserId();

    public string UserName => _principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.USER_NAME) ?? "";

    public string Name => _principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.NAME) ?? "";

    public string PhoneNumber => _principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.PHONE_NUMBER) ?? "";

    public bool PhoneNumberVerified => string.Equals(_principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.PHONE_NUMBER_VERIFIED), "true", StringComparison.InvariantCultureIgnoreCase);

    public string Email => _principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.EMAIL) ?? "";

    public bool EmailVerified => string.Equals(_principalAccessor.Principal?.FindClaimValue(MasaClaimTypes.EMAIL_VERIFIED), "true", StringComparison.InvariantCultureIgnoreCase);

    public Guid? TenantId => _principalAccessor.Principal?.FindTenantId();

    public string Environment => _principalAccessor.Principal?.FindEnvironment() ?? string.Empty;
}
