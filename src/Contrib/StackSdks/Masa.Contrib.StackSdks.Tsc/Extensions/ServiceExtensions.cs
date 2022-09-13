// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Tests")]

namespace Microsoft.Extensions.DependencyInjection;

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
            builder.UseHttpClient(DEFAULT_CLIENT_NAME, options =>
            {
                options.BaseAddress = tscServiceBaseUri;
            });
            builder.Assemblies = new Assembly[] { };
        });

        services.AddSingleton<ITscClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var pmCaching = new TscClient(caller);
            return pmCaching;
        });

        return services;
    }
}
