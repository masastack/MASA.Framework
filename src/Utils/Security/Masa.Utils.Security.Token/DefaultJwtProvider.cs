// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Token;

public class DefaultJwtProvider : IJwtProvider
{
    private readonly JwtConfigurationOptions _options;
    private readonly ILogger<DefaultJwtProvider>? _logger;

    public DefaultJwtProvider(IOptionsSnapshot<JwtConfigurationOptions> options, ILogger<DefaultJwtProvider>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string CreateToken(string value, TimeSpan timeout)
        => CreateToken(new[]
        {
            new Claim(ClaimTypes.Sid, value),
        }, timeout);

    public string CreateToken(Claim[] claims, TimeSpan timeout)
    {
        DateTime notBefore = DateTime.UtcNow;
        DateTime expires = notBefore.Add(timeout);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            notBefore,
            expires,
            credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public bool IsValid(string token, string value, Action<TokenValidationParameters>? action = null)
    {
        var isValid = IsValid(token, out _,
            out ClaimsPrincipal? claimsPrincipal, action);

        return isValid && claimsPrincipal != null && claimsPrincipal.HasClaim(ClaimTypes.Sid, value);
    }

    public bool IsValid(
        string token,
        out SecurityToken? securityToken,
        out ClaimsPrincipal? claimsPrincipal,
        Action<TokenValidationParameters>? action = null)
    {
        securityToken = null;
        claimsPrincipal = null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecurityKey)),
            };
            action?.Invoke(validationParameters);
            claimsPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
            return securityToken != null;
        }
        catch (SecurityTokenException ex)
        {
            _logger?.LogError("... IsValid Failed on SecurityTokenException", ex);
            return false;
        }
        catch (Exception ex)
        {
            _logger?.LogError("... IsValid Failed", ex);
            return false;
        }
    }
}
