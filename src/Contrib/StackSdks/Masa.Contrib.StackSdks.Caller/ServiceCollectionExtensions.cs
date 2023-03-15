// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackCaller(
        this IServiceCollection services,
        Assembly assembly,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptions,
        Action<ClientRefreshTokenOptions>? clientRefreshTokenOptions)
    {
        MasaArgumentException.ThrowIfNull(jwtTokenValidatorOptions);

        services.Configure(jwtTokenValidatorOptions);
        services.Configure(clientRefreshTokenOptions);
        services.AddScoped<TokenProvider>();
        services.AddSingleton<JwtTokenValidator>();
        services.AddAutoRegistrationCaller(assembly);
        return services;
    }
}
