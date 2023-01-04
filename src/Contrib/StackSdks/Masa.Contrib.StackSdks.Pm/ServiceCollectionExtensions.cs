// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPmClient(this IServiceCollection services, string pmServiceBaseAddress)
    {
        MasaArgumentException.ThrowIfNull(pmServiceBaseAddress);

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.BaseAddress = pmServiceBaseAddress;
            });
            callerOptions.DisableAutoRegistration = true;
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Func<Task<string>> pmServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(pmServiceBaseAddressFunc);

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, async builder =>
            {
                builder.BaseAddress = await pmServiceBaseAddressFunc.Invoke();
            }, true);
            callerOptions.DisableAutoRegistration = true;
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Action<CallerOptions> callerOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptions);

        if (services.Any(service => service.ServiceType == typeof(IPmClient)))
            return services;

        services.AddCaller(callerOptions.Invoke);

        services.AddScoped<IPmClient>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var pmCaching = new PmClient(callProvider);
            return pmCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
