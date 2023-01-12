// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    private static void InitializeMasaStackConfiguration(this IServiceCollection services)
    {
        //TODO: Replace IConfiguration data source
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var masaStackConfig = serviceProvider.GetRequiredService<IMasaStackConfig>();

        masaStackConfig.SetValue(MasaStackConfigConst.VERSION, configuration.GetValue<string>(MasaStackConfigConst.VERSION) ?? "");
        masaStackConfig.SetValue(MasaStackConfigConst.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConst.IS_DEMO).ToString());
        masaStackConfig.SetValue(MasaStackConfigConst.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConst.DOMAIN_NAME) ?? "");
        masaStackConfig.SetValue(MasaStackConfigConst.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConst.TLS_NAME) ?? "");

        masaStackConfig.SetValue(MasaStackConfigConst.REDIS, configuration.GetValue<string>(MasaStackConfigConst.REDIS) ?? "");

        var allServer = configuration.GetValue<Dictionary<string, List<string>>>(MasaStackConfigConst.MASA_ALL_SERVER);
        if (allServer != null)
        {
            masaStackConfig.SetValue(MasaStackConfigConst.MASA_ALL_SERVER, JsonSerializer.Serialize(allServer.Keys));
        }


    }

    public static IServiceCollection AddMasaStackConfig(this IServiceCollection services, bool init = false)
    {
        services.TryAddSingleton<IMasaStackConfig>();
        if (init)
        {
            InitializeMasaStackConfiguration(services);
        }
        return services;
    }
}
