// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        services.TryAddSingleton<IMasaStackConfig>();

        //TODO: Replace IConfiguration data source
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var masaStackConfig = serviceProvider.GetRequiredService<IMasaStackConfig>();

        masaStackConfig.SetValue(MasaStackConfigConst.VERSION, configuration.GetValue<string>(MasaStackConfigConst.VERSION) ?? "");
        masaStackConfig.SetValue(MasaStackConfigConst.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConst.IS_DEMO).ToString());
        masaStackConfig.SetValue(MasaStackConfigConst.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConst.DOMAIN_NAME) ?? "");
        masaStackConfig.SetValue(MasaStackConfigConst.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConst.TLS_NAME) ?? "");

        return services;
    }
}
