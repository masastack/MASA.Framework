// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Sso;

public class JwtTokenValidator : IJwtTokenValidator
{
    readonly JwtTokenValidatorOptions _jwtTokenValidatorOptions;
    readonly MasaOpenIdConnectOptions _masaOpenIdConnectOptions;
    readonly HttpClient _httpClient;
    readonly ILogger<JwtTokenValidator> _logger;

    public JwtTokenValidator(
        JwtTokenValidatorOptions jwtTokenValidatorOptions,
        MasaOpenIdConnectOptions masaOpenIdConnectOptions,
        HttpClient httpClient,
        ILogger<JwtTokenValidator> logger)
    {
        _jwtTokenValidatorOptions = jwtTokenValidatorOptions;
        _masaOpenIdConnectOptions = masaOpenIdConnectOptions;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(ClaimsPrincipal?, string accessToken)> ValidateAccessTokenAsync(string accessToken, string? refreshToken = null)
    {
        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(_jwtTokenValidatorOptions.AuthorityEndpoint).ConfigureAwait(false);

        var keys = new List<SecurityKey>();
        foreach (var webKey in discoveryDocument.KeySet.Keys)
        {
            var e = Base64Url.Decode(webKey.E);
            var n = Base64Url.Decode(webKey.N);
            var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
            {
                KeyId = webKey.Kid
            };
            keys.Add(key);
        }
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = _jwtTokenValidatorOptions.ValidateLifetime,
            ValidateAudience = _jwtTokenValidatorOptions.ValidateAudience,
            ValidateIssuer = _jwtTokenValidatorOptions.ValidateIssuer,
            ValidIssuer = discoveryDocument.Issuer,
            ValidAudiences = _jwtTokenValidatorOptions.ValidAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys
        };
        JwtSecurityTokenHandler handler = new();
        handler.InboundClaimTypeMap.Clear();
        ClaimsPrincipal? claimsPrincipal = null;
        try
        {
            claimsPrincipal = handler.ValidateToken(accessToken, validationParameters, out SecurityToken _);
        }
        catch (SecurityTokenExpiredException)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var tokenClient = new TokenClient(_httpClient, new TokenClientOptions
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = _masaOpenIdConnectOptions.ClientId,
                    ClientSecret = _masaOpenIdConnectOptions.ClientSecret
                });
                var tokenResult = await tokenClient.RequestRefreshTokenAsync(refreshToken).ConfigureAwait(false);
                if (tokenResult.IsError)
                {
                    _logger.LogError(tokenResult.Error);
                }
                else
                {
                    return (handler.ValidateToken(tokenResult.AccessToken, validationParameters, out SecurityToken _), tokenResult.AccessToken);
                }
            }
            else
            {
                _logger.LogWarning("RefreshToken is null,please check AllowOfflineAccess value(true) and RequestedScopes should contains offline_access");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "JwtTokenValidator failed");
        }
        return (claimsPrincipal, accessToken);
    }
}
