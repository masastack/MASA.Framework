// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPmClient(this IServiceCollection services, string pmServiceBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(pmServiceBaseAddress))
        {
            throw new ArgumentNullException(nameof(pmServiceBaseAddress));
        }

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(pmServiceBaseAddress);
            });
            callerOptions.DisableAutoRegistration = true;
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));

        if (services.Any(service => service.ServiceType == typeof(IPmClient)))
            return services;

        services.AddCaller(callerOptions.Invoke);

        services.AddSingleton<IPmClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var pmCaching = new PmClient(callProvider);
            return pmCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
