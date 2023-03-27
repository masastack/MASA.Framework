// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAlertClient(this IServiceCollection services, string alertServiceBaseAddress)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(alertServiceBaseAddress);

        return services.AddAlertClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(alertServiceBaseAddress);
            }).UseAuthentication();
        });
    }

    public static IServiceCollection AddAlertClient(this IServiceCollection services, Func<string> alertServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(alertServiceBaseAddressFunc);

        return services.AddAlertClient(callerOptionsBuilder =>
        {
            callerOptionsBuilder
                .UseHttpClient(builder =>
                {
                    builder.BaseAddress = alertServiceBaseAddressFunc.Invoke();
                }).UseAuthentication();
        });
    }

    public static IServiceCollection AddAlertClient(this IServiceCollection services, Action<CallerOptionsBuilder> callerOptionsBuilder)
    {
        MasaArgumentException.ThrowIfNull(callerOptionsBuilder);

        if (services.Any(service => service.ServiceType == typeof(IAlertClient)))
            return services;

        services.AddHttpContextAccessor();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptionsBuilder);

        services.AddScoped<IAlertClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var alertCaching = new AlertClient(caller);
            return alertCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
