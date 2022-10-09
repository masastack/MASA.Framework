// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso;

public interface IIdentityProvider
{
    Task<TokenResult> RequestTokenAsync(TokenProfile tokenProfile);

    Task<TokenRevocationResult> RevokeTokenAsync(string accessToken);

    Task<UserInfoResult> GetUserInfoAsync(string token);
}
