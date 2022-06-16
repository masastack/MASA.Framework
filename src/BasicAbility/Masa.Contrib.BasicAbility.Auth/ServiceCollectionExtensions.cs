// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthClient(this IServiceCollection services, string authServiceBaseAddress)
    {
        ArgumentNullException.ThrowIfNull(authServiceBaseAddress, nameof(authServiceBaseAddress));

        return services.AddAuthClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Name = DEFAULT_CLIENT_NAME;
                builder.Configure = opt => opt.BaseAddress = new Uri(authServiceBaseAddress);
            }).AddHttpMessageHandler<HttpEnvironmentDelegatingHandler>();
        });
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (!services.Any(service => service.ServiceType == typeof(IUserContext)))
        {
            throw new Exception("Please add IUserContext first.");
        }

        services.AddHttpContextAccessor();
        services.AddScoped<HttpEnvironmentDelegatingHandler>();
        services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
        services.AddCaller(callerOptions);

        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var userContext = serviceProvider.GetRequiredService<IUserContext>();
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var authClient = new AuthClient(callProvider, userContext);
            return authClient;
        });

        return services;
    }
}

