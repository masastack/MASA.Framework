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
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(authServiceBaseAddress);
            })
            .AddHttpMessageHandler<HttpEnvironmentDelegatingHandler>();
        });
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));
        if (services.All(service => service.ServiceType != typeof(IMultiEnvironmentUserContext)))
            throw new Exception("Please add IMultiEnvironmentUserContext first.");

        services.AddHttpContextAccessor();
        services.TryAddScoped<IEnvironmentProvider, EnvironmentProvider>();
        services.AddScoped<HttpEnvironmentDelegatingHandler>();
        services.AddCaller(callerOptions);

        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var tokenProvider = serviceProvider.GetRequiredService<TokenProvider>();
            var userContext = serviceProvider.GetRequiredService<IMultiEnvironmentUserContext>();
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            callProvider.ConfigRequestMessage(httpRequestMessage =>
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
            });
            var authClient = new AuthClient(callProvider, userContext);
            return authClient;
        });

        return services;
    }
}
