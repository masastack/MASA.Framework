// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Identity.IdentityModel.BlazorWebAssembly;

public class BlazorCurrentPrincipalAccessor : ICurrentPrincipalAccessor
{
    readonly AuthenticationStateProvider _authenticationStateProvider;

    public BlazorCurrentPrincipalAccessor(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public ClaimsPrincipal? GetCurrentPrincipal()
    {
        return _authenticationStateProvider.GetAuthenticationStateAsync().Result.User;
    }
}
