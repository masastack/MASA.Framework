// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.BlazorWebAssembly;

public class BlazorCurrentPrincipalAccessor : ICurrentPrincipalAccessor
{
    protected MasaComponentsClaimsCache ClaimsCache { get; }

    public BlazorCurrentPrincipalAccessor(IClientScopeServiceProviderAccessor clientScopeServiceProviderAccessor)
    {
        ClaimsCache = clientScopeServiceProviderAccessor.ServiceProvider.GetRequiredService<MasaComponentsClaimsCache>();
    }

    public ClaimsPrincipal? GetCurrentPrincipal()
    {
        return ClaimsCache.Principal;
    }
}
