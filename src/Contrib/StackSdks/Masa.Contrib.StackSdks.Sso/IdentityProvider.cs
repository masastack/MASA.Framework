// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Sso;

public class IdentityProvider : IIdentityProvider
{
    readonly HttpClient _httpClient;
    readonly MasaOpenIdConnectOptions _masaOpenIdConnectOptions;

    public IdentityProvider(HttpClient httpClient, MasaOpenIdConnectOptions masaOpenIdConnectOptions)
    {
        _httpClient = httpClient;
        _masaOpenIdConnectOptions = masaOpenIdConnectOptions;

    }

    async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync()
    {
        return await _httpClient.GetDiscoveryDocumentAsync(_masaOpenIdConnectOptions.Authority);
    }

    public async Task<UserInfoResult> GetUserInfoAsync(string token)
    {
        var disco = await GetDiscoveryDocumentAsync();
        var response = await _httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token = token
        });
        return response.Adapt<UserInfoResult>();
    }

    public async Task<TokenResult> RequestTokenAsync(TokenProfile tokenProfile)
    {
        var disco = await GetDiscoveryDocumentAsync();
        var response = await _httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _masaOpenIdConnectOptions.ClientId,
            ClientSecret = _masaOpenIdConnectOptions.ClientSecret,
            GrantType = tokenProfile.GrantType,
            Parameters = tokenProfile.Parameters.Adapt<Parameters>()
        });
        return response.Adapt<TokenResult>();
    }

    public async Task<TokenRevocationResult> RevokeTokenAsync(string accessToken)
    {
        var disco = await GetDiscoveryDocumentAsync();
        var result = await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
        {
            Address = disco.RevocationEndpoint,
            ClientId = _masaOpenIdConnectOptions.ClientId,
            ClientSecret = _masaOpenIdConnectOptions.ClientSecret,

            Token = accessToken
        });
        return result.Adapt<TokenRevocationResult>();
    }
}
