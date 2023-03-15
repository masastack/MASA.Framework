// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public class JwtTokenValidator
{
    readonly JwtTokenValidatorOptions _jwtTokenValidatorOptions;
    readonly ClientRefreshTokenOptions _clientRefreshTokenOptions;
    readonly HttpClient _httpClient;
    readonly ILogger<JwtTokenValidator> _logger;

    public JwtTokenValidator(
        IOptions<JwtTokenValidatorOptions> jwtTokenValidatorOptions,
        HttpClient httpClient,
        ILogger<JwtTokenValidator> logger,
        IOptions<ClientRefreshTokenOptions> clientRefreshTokenOptions)
    {
        _jwtTokenValidatorOptions = jwtTokenValidatorOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
        _clientRefreshTokenOptions = clientRefreshTokenOptions.Value;
    }

    public async Task ValidateTokenAsync(TokenProvider tokenProvider)
    {
        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(_jwtTokenValidatorOptions.AuthorityEndpoint);
        var validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = _jwtTokenValidatorOptions.ValidateLifetime,
            ValidateAudience = _jwtTokenValidatorOptions.ValidateAudience,
            ValidateIssuer = _jwtTokenValidatorOptions.ValidateIssuer,
            ValidIssuer = discoveryDocument.Issuer,
            ValidAudiences = _jwtTokenValidatorOptions.ValidAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = GetIssuerSigningKeys(discoveryDocument)
        };
        JwtSecurityTokenHandler handler = new();
        handler.InboundClaimTypeMap.Clear();
        try
        {
            handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
        }
        catch (SecurityTokenExpiredException)
        {
            if (!string.IsNullOrEmpty(tokenProvider.RefreshToken))
            {
                var tokenClient = new TokenClient(_httpClient, new TokenClientOptions
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = _clientRefreshTokenOptions.ClientId,
                    ClientSecret = _clientRefreshTokenOptions.ClientSecret
                });
                var tokenResult = await tokenClient.RequestRefreshTokenAsync(tokenProvider.RefreshToken).ConfigureAwait(false);
                if (tokenResult.IsError)
                {
                    _logger?.LogError("JwtTokenValidator failed, RefreshToken failed, Error Message: {Message}", tokenResult.Error);
                    throw new UserFriendlyException($"JwtTokenValidator failed, RefreshToken failed, Error Message: {tokenResult.Error}");
                }
                else
                {
                    tokenProvider.AccessToken = tokenResult.AccessToken;
                    handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
                }
            }
            else
            {
                _logger?.LogWarning(
                    "RefreshToken is null,please AllowOfflineAccess value(true) and RequestedScopes should contains offline_access");
                throw new UserFriendlyException("JwtTokenValidator failed, RefreshToken is null");
            }
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "JwtTokenValidator failed");
            throw new UserFriendlyException("JwtTokenValidator failed");
        }
    }

    private static List<SecurityKey> GetIssuerSigningKeys(DiscoveryDocumentResponse discoveryDocument)
    {
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
        return keys;
    }
}
