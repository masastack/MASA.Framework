// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcClient(this IServiceCollection services, string mcServiceBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(mcServiceBaseAddress))
        {
            throw new ArgumentNullException(nameof(mcServiceBaseAddress));
        }

        return services.AddMcClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.Name = DEFAULT_CLIENT_NAME;
                builder.Configure = opt => opt.BaseAddress = new Uri(mcServiceBaseAddress);
            });
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (services.Any(service => service.ServiceType == typeof(IMcClient)))
            return services;

        services.AddCaller(callerOptions.Invoke);

        services.AddSingleton<IMcClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var mcCaching = new McClient(callProvider);
            return mcCaching;
        });

        return services;
    }
}
