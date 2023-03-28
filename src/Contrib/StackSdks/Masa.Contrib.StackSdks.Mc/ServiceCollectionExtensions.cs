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
            callerOptions.UseHttpClient(builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(mcServiceBaseAddress);
            }).UseAuthentication();
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Func<string> mcServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(mcServiceBaseAddressFunc);

        return services.AddMcClient(callerOptionsBuilder =>
        {
            callerOptionsBuilder
                .UseHttpClient(builder =>
                {
                    builder.BaseAddress = mcServiceBaseAddressFunc.Invoke();
                }).UseAuthentication();
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Action<CallerBuilder> callerOptionsBuilder)
    {
        MasaArgumentException.ThrowIfNull(callerOptionsBuilder);

        if (services.Any(service => service.ServiceType == typeof(IMcClient)))
            return services;

        services.AddHttpContextAccessor();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptionsBuilder);

        services.AddScoped<IMcClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var mcCaching = new McClient(caller);
            return mcCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
