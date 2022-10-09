// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso.Token;

public class TokenResult : ProtocolResult
{
    public string? AccessToken { get; set; }

    public string? IdentityToken { get; set; }

    public string? Scope { get; set; }

    public string? TokenType { get; set; }

    public string? RefreshToken { get; set; }

    public string? ErrorDescription { get; set; }

    public int ExpiresIn { get; set; }
}
