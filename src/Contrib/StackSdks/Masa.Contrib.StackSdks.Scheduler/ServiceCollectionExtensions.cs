// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSchedulerClient(this IServiceCollection services, string schedulerServiceBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(schedulerServiceBaseAddress))
        {
            throw new ArgumentNullException(nameof(schedulerServiceBaseAddress));
        }

        return services.AddSchedulerClient(callerOptions =>
        {
            callerOptions
                .UseHttpClient(builder =>
                {
                    builder.Configure = opt => opt.BaseAddress = new Uri(schedulerServiceBaseAddress);
                }).UseAuthentication();
        });
    }

    public static IServiceCollection AddSchedulerClient(this IServiceCollection services, Action<CallerBuilder> callerOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(callerOptionsBuilder, nameof(callerOptionsBuilder));

        if (services.Any(service => service.ImplementationType == typeof(SchedulerProvider)))
            return services;

        services.AddSingleton<SchedulerProvider>();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptionsBuilder);

        services.AddScoped<ISchedulerClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var schedulerClient = new SchedulerClient(caller);
            return schedulerClient;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class SchedulerProvider
    {
    }
}
