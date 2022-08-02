// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Token;

public interface IJwtProvider
{
    string CreateToken(string value, TimeSpan timeout);

    string CreateToken(Claim[] claims, TimeSpan timeout);

    bool IsValid(
        string token,
        string value,
        Action<TokenValidationParameters>? action = null);

    bool IsValid(
        string token,
        out SecurityToken? securityToken,
        out ClaimsPrincipal? claimsPrincipal,
        Action<TokenValidationParameters>? action = null);
}
