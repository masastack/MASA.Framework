// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSchedulerClient(this IServiceCollection services, string schedulerServiceBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(schedulerServiceBaseAddress))
        {
            throw new ArgumentNullException(nameof(schedulerServiceBaseAddress));
        }

        services.AddSingleton<IRequestMessage, JsonRequestMessage>();
        return services.AddSchedulerClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Name = DEFAULT_CLIENT_NAME;
                builder.Configure = opt => opt.BaseAddress = new Uri(schedulerServiceBaseAddress);
            }).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
        });
    }

    public static IServiceCollection AddSchedulerClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (services.Any(service => service.ImplementationType == typeof(SchedulerProvider)))
            return services;

        services.AddSingleton<SchedulerProvider>();
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddHttpContextAccessor();
        services.AddCaller(callerOptions);

        services.AddScoped<ISchedulerClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var schedulerClient = new SchedulerClient(callProvider, loggerFactory);
            return schedulerClient;
        });

        return services;
    }

    private class SchedulerProvider
    {
    }
}
