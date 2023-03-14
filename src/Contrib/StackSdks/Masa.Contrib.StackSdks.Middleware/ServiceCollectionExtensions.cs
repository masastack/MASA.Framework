// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackMiddleware(this IServiceCollection services)
    {
        services.AddPluggableServices();
        services.AddScoped<DisabledRequestMiddleware>();
        services.AddTransient(typeof(IEventMiddleware<>), typeof(DisabledEventMiddleware<>));
        services.AddTransient(typeof(IEventMiddleware<>), typeof(ValidatorEventMiddleware<>));
        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy("A healthy result."));
        return services;
    }

    private static IServiceCollection AddPluggableServices(this IServiceCollection services)
    {
        services.TryAddScoped<IDisabledEventDeterminer, DefaultDisabledEventDeterminer>();
        services.TryAddScoped<IDisabledRequestDeterminer, DefaultDisabledRequestDeterminer>();
        return services;
    }
}
