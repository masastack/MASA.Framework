// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPmClient(this IServiceCollection services, string pmServiceBaseAddress)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(pmServiceBaseAddress);

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.BaseAddress = pmServiceBaseAddress;
            }).UseAuthentication();
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Func<string> pmServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(pmServiceBaseAddressFunc);

        return services.AddPmClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.BaseAddress = pmServiceBaseAddressFunc.Invoke();
            }).UseAuthentication();
        });
    }

    public static IServiceCollection AddPmClient(this IServiceCollection services, Action<CallerOptionsBuilder> callerOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptions);

        if (services.Any(service => service.ServiceType == typeof(IPmClient)))
            return services;

        services.AddHttpContextAccessor();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptions.Invoke);

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
