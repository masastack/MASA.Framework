// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        services.TryAddSingleton<IMasaStackConfig>();

        //TODO: Replace IConfiguration data source
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        //var keys = configuration.GetChildren();

        var masaStackConfig = serviceProvider.GetRequiredService<IMasaStackConfig>();
        //SsoDomain 
        masaStackConfig.SetValue("SsoDomain", configuration["SsoDomain"] ?? "");

        return services;
    }
}
