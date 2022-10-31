// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class TokenModel
{
    public string AccessToken { get; set; }

    public string IdentityToken { get; set; }

    public string RefreshToken { get; set; }

    public int ExpiresIn { get; set; }
}
