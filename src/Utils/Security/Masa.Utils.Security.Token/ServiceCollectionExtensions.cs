// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, Action<JwtConfigurationOptions> options)
    {
        services.Configure(options);
        services.TryAddScoped<IJwtProvider, DefaultJwtProvider>();
        _ = new JwtUtils(services);
        return services;
    }
}
