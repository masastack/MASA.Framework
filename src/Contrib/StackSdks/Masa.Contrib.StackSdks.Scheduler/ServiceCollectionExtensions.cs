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
            callerOptions.UseHttpClient(builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(schedulerServiceBaseAddress);
            });
        });
    }

    public static IServiceCollection AddSchedulerClient(this IServiceCollection services, Action<CallerOptionsBuilder> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (services.Any(service => service.ImplementationType == typeof(SchedulerProvider)))
            return services;

        services.AddSingleton<SchedulerProvider>();
        services.AddHttpContextAccessor();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptions);

        services.AddScoped<ISchedulerClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            if (caller is ICallerExpand callerExpand)
            {
                callerExpand.ConfigRequestMessage(httpRequestMessage =>
                {
                    var tokenProvider = serviceProvider.GetService<TokenProvider>();
                    if (tokenProvider != null)
                    {
                        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
                    }
                    return Task.CompletedTask;
                });
            }
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
