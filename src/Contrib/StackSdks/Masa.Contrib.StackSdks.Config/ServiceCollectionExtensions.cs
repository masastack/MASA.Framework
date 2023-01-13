// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static void InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        services.Configure<MasaStackConfigOptions>(masaStackConfig =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            masaStackConfig.SetValue(MasaStackConfigConst.VERSION, configuration.GetValue<string>(MasaStackConfigConst.VERSION) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConst.IS_DEMO).ToString());
            masaStackConfig.SetValue(MasaStackConfigConst.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConst.DOMAIN_NAME) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConst.TLS_NAME) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.CLUSTER, configuration.GetValue<string>(MasaStackConfigConst.CLUSTER) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConst.OTLP_URL) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.REDIS, configuration.GetValue<string>(MasaStackConfigConst.REDIS) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConst.CONNECTIONSTRING) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.MASA_ALL_SERVER, configuration.GetValue<string>(MasaStackConfigConst.MASA_ALL_SERVER) ?? "");
            masaStackConfig.SetValue(MasaStackConfigConst.ELASTIC, configuration.GetValue<string>(MasaStackConfigConst.ELASTIC) ?? "");
        });
    }


    public static IServiceCollection AddMasaStackConfig(this IServiceCollection services, bool init = true)
    {
        services.TryAddSingleton<IMasaStackConfig, MasaStackConfig>();
        if (init)
        {
            InitializeMasaStackConfiguration(services);
        }
        return services;
    }

    public static IMasaStackConfig GetMasaStackConfig(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IMasaStackConfig>();
    }
}
