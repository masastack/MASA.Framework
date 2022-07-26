// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.BasicAbility.Tsc.Tests")]
namespace Masa.Contrib.BasicAbility.Tsc;

public static class ServiceExtensions
{
    private const string DEFAULT_CLIENT_NAME = "masa.contrib.basicability.tsc";

    public static IServiceCollection AddTscClient(this IServiceCollection services, string tscServiceBaseUri)
    {
        ArgumentNullException.ThrowIfNull(tscServiceBaseUri, nameof(tscServiceBaseUri));

        if (services.Any(service => service.ServiceType == typeof(ITscClient)))
            return services;

        services.AddCaller(builder =>
        {
            builder.UseHttpClient(options =>
            {
                options.BaseAddress = tscServiceBaseUri;
                options.Name = DEFAULT_CLIENT_NAME;
            });
        });

        services.AddSingleton<ITscClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME);
            var pmCaching = new TscClient(caller);
            return pmCaching;
        });

        return services;
    }
}
