// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcClient(this IServiceCollection services, string mcServiceBaseAddress)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(mcServiceBaseAddress);

        return services.AddMcClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(mcServiceBaseAddress);
            }).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
            callerOptions.DisableAutoRegistration = true;
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Func<Task<string>> mcServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(mcServiceBaseAddressFunc);

        return services.AddMcClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, async builder =>
            {
                builder.BaseAddress = await mcServiceBaseAddressFunc.Invoke();
            }, true).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
            callerOptions.DisableAutoRegistration = true;
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptions);

        if (services.Any(service => service.ServiceType == typeof(IMcClient)))
            return services;

        services.AddHttpContextAccessor();
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddCaller(callerOptions);

        services.AddScoped<IMcClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var mcCaching = new McClient(callProvider);
            return mcCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
