// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Sso;

public interface IJwtTokenValidator
{
    Task<(ClaimsPrincipal?, string accessToken)> ValidateAccessTokenAsync(string accessToken, string? refreshToken = null);
}
