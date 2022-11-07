// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.BlazorServer;

public class BlazorCurrentPrincipalAccessor : ICurrentPrincipalAccessor
{
    readonly AuthenticationStateProvider _authenticationStateProvider;
    readonly IHttpContextAccessor _httpContextAccessor;

    public BlazorCurrentPrincipalAccessor(AuthenticationStateProvider authenticationStateProvider, IHttpContextAccessor httpContextAccessor)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? GetCurrentPrincipal()
    {
        //https://github.com/dotnet/aspnetcore/issues/28684
        return _httpContextAccessor.HttpContext?.User ?? _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false).GetAwaiter().GetResult().User;
    }
}
