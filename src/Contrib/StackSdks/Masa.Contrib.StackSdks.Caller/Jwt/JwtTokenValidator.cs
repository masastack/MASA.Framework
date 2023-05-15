// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public class JwtTokenValidator
{
    readonly JwtTokenValidatorOptions _jwtTokenValidatorOptions;
    readonly ClientRefreshTokenOptions _clientRefreshTokenOptions;
    readonly HttpClient _httpClient;
    readonly ILogger<JwtTokenValidator>? _logger;

    public JwtTokenValidator(
        IOptions<JwtTokenValidatorOptions> jwtTokenValidatorOptions,
        HttpClient httpClient,
        IHttpClientFactory httpClientFactory,
        IOptions<ClientRefreshTokenOptions> clientRefreshTokenOptions,
        ILogger<JwtTokenValidator>? logger)
    {
        _jwtTokenValidatorOptions = jwtTokenValidatorOptions.Value;
        _httpClient = httpClientFactory.CreateClient(Constant.HTTP_NAME);
        _clientRefreshTokenOptions = clientRefreshTokenOptions.Value;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(TokenProvider tokenProvider)
    {
        var discoveryDocument = await GetDiscoveryDocument();
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
        ClaimsPrincipal? claimsPrincipal = null;
        try
        {
            claimsPrincipal = handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
        }
        catch (SecurityTokenExpiredException)
        {
            var tokenResult = await RefreshTokenAsync(tokenProvider.RefreshToken);
            if (tokenResult != null)
            {
                if (tokenResult.IsError)
                {
                    _logger?.LogError("JwtTokenValidator failed, RefreshToken failed, Error Message: {Message}", tokenResult.Error);
                }
                else
                {
                    tokenProvider.AccessToken = tokenResult.AccessToken;
                    return handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
                }
            }
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "JwtTokenValidator failed");
            throw new UserFriendlyException("JwtTokenValidator failed");
        }
        return claimsPrincipal;
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger?.LogWarning("RefreshToken is null,please AllowOfflineAccess value(true) and RequestedScopes should contains offline_access");
            throw new ArgumentNullException(nameof(refreshToken));
        }
        if (string.IsNullOrEmpty(_clientRefreshTokenOptions.ClientId))
        {
            _logger?.LogWarning("ClientRefreshTokenOptions.ClientId is empty,refresh token no work");
            return null;
        }
        var tokenEndpoint = (await GetDiscoveryDocument()).TokenEndpoint;
        var tokenClient = new TokenClient(_httpClient, new TokenClientOptions
        {
            Address = tokenEndpoint,
            ClientId = _clientRefreshTokenOptions.ClientId,
            ClientSecret = _clientRefreshTokenOptions.ClientSecret
        });
        var tokenResult = await tokenClient.RequestRefreshTokenAsync(refreshToken).ConfigureAwait(false);
        return tokenResult;
    }

    private async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
    {
        return await _httpClient.GetDiscoveryDocumentAsync(_jwtTokenValidatorOptions.AuthorityEndpoint);
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
