// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

/// <summary>
/// Used to verify Token, the declaration cycle is a singleton
/// </summary>
public interface ITokenValidatorHandler
{
    Task ValidateTokenAsync(TokenProvider tokenProvider);
}
