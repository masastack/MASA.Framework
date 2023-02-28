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
            });
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Func<string> mcServiceBaseAddressFunc)
    {
        MasaArgumentException.ThrowIfNull(mcServiceBaseAddressFunc);

        return services.AddMcClient(callerOptions =>
        {
            callerOptions.UseHttpClient(builder =>
            {
                builder.BaseAddress = mcServiceBaseAddressFunc.Invoke();
            });
        });
    }

    public static IServiceCollection AddMcClient(this IServiceCollection services, Action<CallerOptionsBuilder> callerOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptions);

        if (services.Any(service => service.ServiceType == typeof(IMcClient)))
            return services;

        services.AddHttpContextAccessor();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptions);

        services.AddScoped<IMcClient>(serviceProvider =>
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
            var mcCaching = new McClient(caller);
            return mcCaching;
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
