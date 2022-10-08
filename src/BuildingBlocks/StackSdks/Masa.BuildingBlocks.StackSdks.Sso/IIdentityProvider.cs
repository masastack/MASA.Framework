// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso;

internal interface IIdentityProvider
{
    Task<TokenResponse> RequestTokenAsync(TokenRequest tokenRequest);

    Task<TokenRevocationResponse> RevokeTokenAsync(TokenRevocationRequest tokenRevocationRequest);
}
