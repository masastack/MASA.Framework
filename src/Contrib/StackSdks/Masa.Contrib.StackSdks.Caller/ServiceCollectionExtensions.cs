// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackCaller(
        this IServiceCollection services,
        Assembly assembly,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptions,
        Action<ClientRefreshTokenOptions>? clientRefreshTokenOptions = null)
    {
        MasaArgumentException.ThrowIfNull(jwtTokenValidatorOptions);

        services.AddHttpClient(Constant.HTTP_NAME).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = delegate { return true; }
        });

        services.Configure(jwtTokenValidatorOptions);
        services.Configure(clientRefreshTokenOptions);
        services.TryAddScoped<ITokenGenerater, DefaultTokenGenerater>();
        services.TryAddScoped(s => s.GetRequiredService<ITokenGenerater>().Generater());
        services.AddSingleton<JwtTokenValidator>();
        services.AddAutoRegistrationCaller(assembly);

        return services;
    }
}
