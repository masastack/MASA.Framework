// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

public static class AuthenticationOptionsExtensions
{
    public static void UseJwtBearer(this AuthenticationOptions authenticationOptions,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptionsAction,
        Action<ClientRefreshTokenOptions> clientRefreshTokenOptionsAction)
    {
        authenticationOptions.Services.AddHttpClient();
        authenticationOptions.Services.AddSingleton<ITokenValidatorHandler, JwtTokenValidatorHandler>();
        authenticationOptions.Services.Configure(jwtTokenValidatorOptionsAction);
        authenticationOptions.Services.Configure(clientRefreshTokenOptionsAction);
    }
}
