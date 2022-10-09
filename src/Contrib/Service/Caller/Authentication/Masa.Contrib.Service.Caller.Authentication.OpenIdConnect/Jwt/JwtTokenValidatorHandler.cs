// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect.Jwt;

public class JwtTokenValidatorHandler : ITokenValidatorHandler
{
    private readonly JwtTokenValidatorOptions _jwtTokenValidatorOptions;
    private readonly ClientRefreshTokenOptions _clientRefreshTokenOptions;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JwtTokenValidatorHandler>? _logger;

    public JwtTokenValidatorHandler(IOptions<JwtTokenValidatorOptions> jwtTokenValidatorOptions,
        IOptions<ClientRefreshTokenOptions> clientRefreshTokenOptions,
        HttpClient httpClient,
        ILogger<JwtTokenValidatorHandler>? logger)
    {
        _jwtTokenValidatorOptions = jwtTokenValidatorOptions.Value;
        _clientRefreshTokenOptions = clientRefreshTokenOptions.Value;
        _httpClient = httpClient;
        _logger = logger;
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
                    _logger?.LogError(tokenResult.Error);
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

    private List<SecurityKey> GetIssuerSigningKeys(DiscoveryDocumentResponse discoveryDocument)
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
